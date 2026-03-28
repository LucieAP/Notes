import axios, { AxiosError, AxiosInstance, type CreateAxiosDefaults } from "axios";
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

export function setupInterceptors(api: AxiosInstance) {
  let isRefreshing = false;

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

  // Динамически подставляем токен перед каждым запросом
  // request.use() добавляет функцию, которая запускается перед отправкой любого запроса
  // config - объект с настройками текущего запроса (url, method, headers и т.д.)
  api.interceptors.request.use((config) => {
    const token = getAccessToken();

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
        | (AxiosError["config"] & { _retry?: boolean })
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
          const res = await api.post<GetTokenResponse, GetTokenResponse>(
            "/auth/refresh",
          );
          const newToken = res.token;

          // сохраняем токен
          setAccessToken(newToken);
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
        }
      }

      return Promise.reject(normalizeError(error));
    },
  );
}

setupInterceptors(api);

export default api;
