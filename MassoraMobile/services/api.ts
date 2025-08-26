import * as SecureStore from 'expo-secure-store';
import { API_CONFIG } from '../constants/config';

class ApiService {
  private async getAuthHeaders() {
    const accessToken = await SecureStore.getItemAsync('access_token');
    return {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${accessToken}`,
    };
  }

  // Token yenileme işlemi
  private async refreshToken() {
    try {
      const refreshToken = await SecureStore.getItemAsync('refresh_token');
      if (!refreshToken) {
        throw new Error('Refresh token bulunamadı');
      }

      console.log('Refreshing token...');
      const response = await fetch(`${API_CONFIG.authBaseUrl}/connect/token`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: new URLSearchParams({
          grant_type: 'refresh_token',
          client_id: 'mobil-hafriyat-client',
          refresh_token: refreshToken,
        }),
      });

      if (response.ok) {
        const tokenData = await response.json();
        console.log('Token refresh successful');
        
        // Yeni token'ları sakla
        await SecureStore.setItemAsync('access_token', tokenData.access_token);
        
        // Refresh token'ı güncelle (OneTimeOnly olduğu için yeni refresh token gelir)
        if (tokenData.refresh_token) {
          await SecureStore.setItemAsync('refresh_token', tokenData.refresh_token);
        } else {
          // Yeni refresh token gelmezse eski token'ı sil
          await SecureStore.deleteItemAsync('refresh_token');
        }
        
        return tokenData.access_token;
      } else {
        const errorText = await response.text();
        console.error('Token refresh failed:', response.status, errorText);
        throw new Error('Token yenilenemedi');
      }
    } catch (error) {
      console.error('Token refresh failed:', error);
      // Refresh token geçersizse temizle
      await SecureStore.deleteItemAsync('refresh_token');
      throw error;
    }
  }

  // API isteği yapma (token yenileme ile)
  private async makeAuthenticatedRequest(url: string, options: RequestInit = {}) {
    let headers = await this.getAuthHeaders();
    
    try {
      const response = await fetch(url, {
        ...options,
        headers: { ...headers, ...options.headers },
      });

      if (response.status === 401) {
        // Token geçersiz, yenilemeyi dene
        const newToken = await this.refreshToken();
        headers = {
          ...headers,
          'Authorization': `Bearer ${newToken}`,
        };
        
        // Yeniden istek yap
        const retryResponse = await fetch(url, {
          ...options,
          headers: { ...headers, ...options.headers },
        });
        
        if (!retryResponse.ok) {
          throw new Error(`API request failed: ${retryResponse.status}`);
        }
        
        return retryResponse;
      }

      if (!response.ok) {
        throw new Error(`API request failed: ${response.status}`);
      }

      return response;
    } catch (error) {
      console.error('API request failed:', error);
      throw error;
    }
  }

  async getVehicles() {
    console.log("getVehicles çalıştı");
    try {
      const response = await this.makeAuthenticatedRequest(
        `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.vehicles}`
      );
      console.log("getVehicles response status:", response.status); // Bunu ekle
      const data = await response.json();
      console.log("getVehicles response data:", data); // Bunu ekle
      return data;
    } catch (error) {
      console.log("getVehicles hata aldı");
      console.error("Get vehicles error:", error);
      throw error;
    }
  }

  // YENİ: Sürücü listesini getir
  async getDrivers() {
    try {
      // API_CONFIG dosyanıza bir drivers endpoint'i eklemeniz gerekebilir
      // Örnek: drivers: '/api/driver/for-dropdown'
      const response = await this.makeAuthenticatedRequest(`${API_CONFIG.baseUrl}${API_CONFIG.endpoints.drivers}`);
      return await response.json();
    } catch (error) {
      console.error('Get drivers error:', error);
      throw error;
    }
  }

  // YENİ: Tam bir çalışma geçmişi kaydı ekle
  async addWorkHistory(payload: any) {
    try {
      const response = await this.makeAuthenticatedRequest(`${API_CONFIG.baseUrl}${API_CONFIG.endpoints.workHistories}`, {
        method: 'POST',
        body: JSON.stringify(payload),
      });
      return await response.json();
    } catch (error) {
      console.error('Add work history error:', error);
      throw error;
    }
  }

  // Çalışma başlat
  async startWork(vehicleId: number, startTime: string) {
    try {
      const response = await this.makeAuthenticatedRequest(`${API_CONFIG.baseUrl}${API_CONFIG.endpoints.workHistories}`, {
        method: 'POST',
        body: JSON.stringify({
          vehicleId,
          startTime,
        }),
      });
      return await response.json();
    } catch (error) {
      console.error('Start work error:', error);
      throw error;
    }
  }

  // Çalışma durdur
  async stopWork(workHistoryId: number, endTime: string) {
    try {
      const response = await this.makeAuthenticatedRequest(`${API_CONFIG.baseUrl}${API_CONFIG.endpoints.workHistories}/${workHistoryId}`, {
        method: 'PUT',
        body: JSON.stringify({
          endTime,
        }),
      });
      return await response.json();
    } catch (error) {
      console.error('Stop work error:', error);
      throw error;
    }
  }

  // Yakıt kaydı ekle
  async addFuel(vehicleId: number, liters: number, cost: number) {
    try {
      const response = await this.makeAuthenticatedRequest(`${API_CONFIG.baseUrl}${API_CONFIG.endpoints.fuelHistories}`, {
        method: 'POST',
        body: JSON.stringify({
          vehicleId,
          liters,
          cost,
        }),
      });
      return await response.json();
    } catch (error) {
      console.error('Add fuel error:', error);
      throw error;
    }
  }

  // Kullanıcı bilgilerini getir (Auth sunucusundan)
  async getUserInfo() {
    try {
      const accessToken = await SecureStore.getItemAsync('access_token');
      const response = await fetch(`${API_CONFIG.authBaseUrl}/connect/userinfo`, {
        headers: {
          'Authorization': `Bearer ${accessToken}`,
        },
      });

      if (response.ok) {
        return await response.json();
      }
      throw new Error('Kullanıcı bilgileri alınamadı');
    } catch (error) {
      console.error('Get user info error:', error);
      throw error;
    }
  }
}

export const apiService = new ApiService(); 