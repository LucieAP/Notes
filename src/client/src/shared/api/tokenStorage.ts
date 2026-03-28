let _accessToken: string | null = null;

export const setAccessToken = (token: string) => {
  _accessToken = token;
};

export const getAccessToken = () => {
  return _accessToken;
};

export const clearAccessToken = () => {
  _accessToken = null;
};
