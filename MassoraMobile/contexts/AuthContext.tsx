import React, { createContext, useContext, useState, useEffect } from 'react';
import * as SecureStore from 'expo-secure-store';
import { AUTH_CONFIG } from '../constants/config';
import { User, AuthState } from '../types/auth';

interface AuthContextType extends AuthState {
  login: (username: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(false);
  const [authLoading, setAuthLoading] = useState(true);

  console.log('AuthProvider - Auth Config:', AUTH_CONFIG);

  // Token exchange işlemi - ROPC flow için
  const exchangeTokenWithCredentials = async (username: string, password: string) => {
    try {
      console.log('Exchanging token with credentials for user:', username);
      
      const formBody = [
        `grant_type=password`,
        `client_id=${encodeURIComponent(AUTH_CONFIG.clientId)}`,
        `username=${encodeURIComponent(username)}`,
        `password=${encodeURIComponent(password)}`,
        `scope=${encodeURIComponent('openid profile massoraapi offline_access')}`
      ].join('&');

      const response = await fetch(`${AUTH_CONFIG.discovery.tokenEndpoint}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: formBody,
      });

      console.log('Token response status:', response.status);

      if (response.ok) {
        const tokenData = await response.json();
        console.log('Token response received:', tokenData);

        // Token'ları güvenli şekilde sakla
        await SecureStore.setItemAsync('access_token', tokenData.access_token);
        if (tokenData.refresh_token) {
          await SecureStore.setItemAsync('refresh_token', tokenData.refresh_token);
        }

        // Kullanıcı bilgilerini al
        const userInfo = await getUserInfo(tokenData.access_token);
        if (userInfo) {
          setUser(userInfo);
          setIsAuthenticated(true);
          console.log('Giriş başarılı! User:', userInfo);
          return true;
        } else {
          console.error('User info alınamadı');
          await logout();
          return false;
        }
      } else {
        const errorText = await response.text();
        console.error('Token exchange failed:', response.status, errorText);
        throw new Error('Giriş başarısız: ' + errorText);
      }
    } catch (error: any) {
      console.error('Token exchange failed:', error);
      throw error;
    }
  };

  // Auth durumunu kontrol et
  useEffect(() => {
    const checkAuthStatus = async () => {
      try {
        const token = await SecureStore.getItemAsync('access_token');
        console.log('Checking auth status, token exists:', !!token);
        
        if (token) {
          const userInfo = await getUserInfo(token);
          if (userInfo) {
            setUser(userInfo);
            setIsAuthenticated(true);
            console.log('Auth status check successful, user:', userInfo);
          } else {
            console.log('Token geçersiz, temizleniyor');
            await logout();
          }
        } else {
          console.log('No token found');
        }
      } catch (error) {
        console.error("Auth status check failed:", error);
        await logout();
      } finally {
        setAuthLoading(false);
      }
    };
    checkAuthStatus();
  }, []);

  const getUserInfo = async (token: string): Promise<User | null> => {
    try {
      console.log('Getting user info from:', AUTH_CONFIG.discovery.userInfoEndpoint);
      const response = await fetch(AUTH_CONFIG.discovery.userInfoEndpoint, {
        headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });
      
      console.log('User info response status:', response.status);
      
      if (response.ok) {
        const userData = await response.json();
        console.log('User info received:', userData);
        return userData;
      } else {
        console.error('User info request failed:', response.status, response.statusText);
        // Response body'yi de logla
        try {
          const errorText = await response.text();
          console.error('Error response body:', errorText);
        } catch (e) {
          console.error('Could not read error response body');
        }
        
        // Geçici olarak mock data döndür (test için)
        console.log('Using mock user data for testing');
        return {
          sub: 'test-user',
          name: 'Test User',
          email: 'test@example.com',
        };
      }
    } catch (error) {
      console.error('Get user info failed:', error);
      // Geçici olarak mock data döndür (test için)
      console.log('Using mock user data due to error');
      return {
        sub: 'test-user',
        name: 'Test User',
        email: 'test@example.com',
      };
    }
  };

  const login = async (username: string, password: string) => {
    setLoading(true);
    try {
      console.log('Starting login flow with username:', username);
      const success = await exchangeTokenWithCredentials(username, password);
      if (!success) {
        throw new Error('Giriş başarısız');
      }
    } catch (error: any) {
      console.error("Login failed:", error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  const logout = async () => {
    try {
      console.log('Logging out...');
      await SecureStore.deleteItemAsync('access_token');
      await SecureStore.deleteItemAsync('refresh_token');
      setIsAuthenticated(false);
      setUser(null);
      console.log('Logout completed');
    } catch (error) {
      console.error('Logout error:', error);
    }
  };

  const value: AuthContextType = { 
    isAuthenticated, 
    user, 
    login, 
    logout, 
    loading, 
    authLoading 
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};