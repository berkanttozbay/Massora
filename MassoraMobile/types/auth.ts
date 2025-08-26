export interface User {
  sub: string;
  name?: string;
  email?: string;
  given_name?: string;
  family_name?: string;
  [key: string]: any;
}

export interface AuthTokens {
  access_token: string;
  refresh_token?: string;
  expires_in?: number;
  token_type?: string;
}

export interface AuthState {
  isAuthenticated: boolean;
  user: User | null;
  loading: boolean;
  authLoading: boolean;
} 