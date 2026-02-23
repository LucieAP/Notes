import axios, { type CreateAxiosDefaults } from "axios";

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

// Динамически подставляем токен перед каждым запросом
api.interceptors.request.use(async (config) => {
  const token = await cookieStore.get("token"); // cookieStore.get тоже async!
  if (token) {
    config.headers.Authorization = `Bearer ${token.value}`;
  }
  return config;
});

// Перехватчик для обработки ошибок
api.interceptors.response.use(
  (response) => response.data,
  (error) => {
    const serverError = error.response?.data?.title || "Server Error";
    return Promise.reject({
      message: serverError,
      status: error.response?.status,
      data: error.response?.data,
    });
  },
);

export default api;
