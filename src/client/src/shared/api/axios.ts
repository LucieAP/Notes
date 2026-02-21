import axios, { type CreateAxiosDefaults } from "axios";

const BASE_URL = import.meta.env.VITE_BASE_URL;

const config: CreateAxiosDefaults = {
  baseURL: BASE_URL,
  withCredentials: true,
};

// Экземпляр Axios с базовыми настройками
const api = axios.create(config);

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
