import axios, {
  AxiosError,
  AxiosInstance,
  type CreateAxiosDefaults,
} from "axios";
import {
  clearAccessToken,
  getAccessToken,
  setAccessToken,
} from "./tokenStorage";
import type { GetTokenResponse } from "./token";

const BASE_URL = import.meta.env.VITE_BASE_URL;

const config: CreateAxiosDefaults = {
  baseURL: BASE_URL,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
};

// Экземпляр Axios с базовыми настройками
const api = axios.create(config);

type QueueItem = {
  resolve: (token: string) => void;
  reject: (error: unknown) => void;
};

function normalizeError(error: unknown) {
  if (axios.isAxiosError(error)) {
    return {
      message: error.response?.data?.title || "Server Error",
      status: error.response?.status,
      data: error.response?.data,
    };
  }

  return {
    message: "Server Error",
    status: undefined,
    data: undefined,
  };
}

function setAuthHeader(requestConfig: any, token: string) {
  requestConfig.headers = requestConfig.headers ?? {};
  requestConfig.headers.Authorization = `Bearer ${token}`;
}

function decodeJwtExp(token: string): number | null {
  try {
    const payload = token.split(".")[1];
    if (!payload) return null;

    const base64 = payload.replace(/-/g, "+").replace(/_/g, "/");
    const json = decodeURIComponent(
      atob(base64)
        .split("")
        .map((char) => `%${char.charCodeAt(0).toString(16).padStart(2, "0")}`)
        .join(""),
    );

    const parsed = JSON.parse(json) as { exp?: unknown };
    return typeof parsed.exp === "number" ? parsed.exp : null;
  } catch {
    return null;
  }
}

function isTokenExpiredOrNearExpiry(token: string, leewaySeconds = 15): boolean {
  const exp = decodeJwtExp(token);
  if (!exp) {
    return false;
  }

  const nowInSeconds = Math.floor(Date.now() / 1000);
  return exp <= nowInSeconds + leewaySeconds;
}

export function setupInterceptors(api: AxiosInstance) {
  let isRefreshing = false;
  let lastRefreshToken: string | null = null;
  let refreshPromise: Promise<string> | null = null;

  // очередь запросов на /refresh
  let refreshQueue: QueueItem[] = [];

  // Отправляем токены всем запросам в очереди и очищаем очередь
  function onRefreshed(newToken: string) {
    refreshQueue.forEach(({ resolve }) => resolve(newToken));
    refreshQueue = [];
  }

  function onRefreshFailed(error: unknown) {
    refreshQueue.forEach(({ reject }) => reject(error));
    refreshQueue = [];
  }

  async function refreshAccessToken(): Promise<string> {
    if (!refreshPromise) {
      refreshPromise = axios
        .post<GetTokenResponse>(`${BASE_URL}/auth/refresh`, undefined, {
          withCredentials: true,
          headers: {
            "Content-Type": "application/json",
          },
        })
        .then((response) => {
          const newToken = response.data.token;
          setAccessToken(newToken);
          return newToken;
        })
        .finally(() => {
          refreshPromise = null;
        });
    }

    return refreshPromise;
  }

  // Динамически подставляем токен перед отправкой каждого запроса
  // config - объект с настройками текущего запроса (url, method, headers и т.д.)
  api.interceptors.request.use(async (config) => {
    const requestUrl = config.url ?? "";
    const isRefreshRequest = requestUrl.includes("/auth/refresh");

    let token = getAccessToken();

    if (!isRefreshRequest && token && isTokenExpiredOrNearExpiry(token)) {
      try {
        token = await refreshAccessToken();
      } catch {
        clearAccessToken();
      }
    }

    if (token) {
      setAuthHeader(config, token);
    }

    return config;
  });

  // Перехватчик для обработки ошибок + refresh логика
  api.interceptors.response.use(
    (response) => response.data,
    async (error: AxiosError) => {
      const originalRequest = error.config as
        | (AxiosError["config"] & { _retry?: boolean }) // устанавливаем свойство _retry
        | undefined;

      if (!originalRequest) {
        return Promise.reject(normalizeError(error));
      }

      const requestUrl = originalRequest.url ?? "";
      const isRefreshRequest = requestUrl.includes("/auth/refresh");

      // проверяем 401
      if (
        error.response?.status === 401 &&
        !originalRequest._retry &&
        !isRefreshRequest
      ) {
        originalRequest._retry = true;

        // если уже идёт refresh — подписываемся
        if (isRefreshing) {
          return new Promise<string>((resolve, reject) => {
            refreshQueue.push({ resolve, reject });
          }).then((token) => {
            setAuthHeader(originalRequest, token);
            return api.request(originalRequest);
          });
        }

        // запускаем refresh
        isRefreshing = true;

        try {
          const newToken = await refreshAccessToken();
          lastRefreshToken = newToken;

          onRefreshed(newToken);

          // Обновляем заголовок Authorization в конфиге оригинального запроса перед повтором
          setAuthHeader(originalRequest, newToken);
          return api.request(originalRequest);
        } catch (refreshError) {
          onRefreshFailed(refreshError);
          clearAccessToken();

          if (window.location.pathname !== "/login") {
            window.location.href = "/login";
          }

          return Promise.reject(normalizeError(refreshError));
        } finally {
          isRefreshing = false;
          // если в окно между onRefreshed и return api.request(originalRequest); в очередь попадет еще один запрос с 401
          if (lastRefreshToken && refreshQueue.length > 0) {
            onRefreshed(lastRefreshToken);
          }
          lastRefreshToken = null;
        }
      }

      return Promise.reject(normalizeError(error));
    },
  );
}

setupInterceptors(api);

export default api;
