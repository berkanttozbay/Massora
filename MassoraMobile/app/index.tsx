import { useEffect } from 'react';
import { Redirect } from 'expo-router';
import { useAuth } from '../contexts/AuthContext';
import { View, ActivityIndicator, Text } from 'react-native';

export default function Index() {
  const { isAuthenticated, authLoading, user } = useAuth();

  console.log('Index - Auth state:', { isAuthenticated, authLoading, user: !!user });

  if (authLoading) {
    return (
      <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
        <ActivityIndicator size="large" color="#2196F3" />
        <Text style={{ marginTop: 16, color: '#666' }}>Kimlik doğrulama kontrol ediliyor...</Text>
      </View>
    );
  }

  if (isAuthenticated && user) {
    console.log('User authenticated, redirecting to home');
    return <Redirect href="/(app)/home" />;
  } else {
    console.log('User not authenticated, redirecting to login');
    return <Redirect href="/(auth)/login" />;
  }
} 