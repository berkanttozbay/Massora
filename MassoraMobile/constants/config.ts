// Auth Sunucusu Konfigürasyonu
export const AUTH_CONFIG = {
  issuer: 'http://192.168.1.42:5139',
  clientId: 'mobil-hafriyat-client',
  redirectUrl: 'com.massora.hafriyatapp:/oauth2redirect',
  scopes: ['openid', 'profile', 'massoraapi', 'offline_access'],
  discovery: {
    authorizationEndpoint: 'http://192.168.1.42:5139/connect/authorize',
    tokenEndpoint: 'http://192.168.1.42:5139/connect/token',
    userInfoEndpoint: 'http://192.168.1.42:5139/connect/userinfo',
  },
};

// API Konfigürasyonu
export const API_CONFIG = {
  baseUrl: 'http://192.168.1.42:5260',
  authBaseUrl: 'http://192.168.1.42:5139',
  endpoints: {
    vehicles: '/api/vehicle/for-dropdown',
    workHistories: '/api/workhistories',
    fuelHistories: '/api/fuelhistories',
    drivers: '/api/driver/for-dropdown',
  },
};

// Uygulama Konfigürasyonu
export const APP_CONFIG = {
  name: 'HafriyatProjem',
  version: '1.0.0',
  scheme: 'com.massora.hafriyatapp',
}; 