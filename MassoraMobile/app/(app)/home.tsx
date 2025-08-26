import React from 'react';
import { View, StyleSheet } from 'react-native';
import { Button, Appbar, Text, Avatar } from 'react-native-paper';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Link, useRouter } from 'expo-router';
import { useAuth } from '../../contexts/AuthContext';

export default function HomeScreen() {
  const { user, logout } = useAuth();
  const router = useRouter();

  const handleLogout = async () => {
    try {
      await logout();
      router.replace('/(auth)/login');
    } catch (error) {
      console.error('Logout error:', error);
    }
  };

  return (
    <SafeAreaView style={styles.container}>
      <Appbar.Header>
        <Appbar.Content title="HafriyatProjem" />
        <Appbar.Action icon="logout" onPress={handleLogout} />
      </Appbar.Header>
      
      <View style={styles.content}>
        {/* Kullanıcı Bilgileri */}
        <View style={styles.userSection}>
          <Avatar.Text size={64} label={user?.name?.charAt(0) || 'U'} />
          <Text style={styles.welcomeText}>
            Hoş Geldin, {user?.name || 'Kullanıcı'}
          </Text>
        </View>

        {/* Ana Butonlar */}
        <View style={styles.buttonContainer}>
          <Link href="/(app)/work-tracking" asChild>
            <Button 
              mode="contained" 
              style={styles.button}
              contentStyle={styles.buttonContent}
              labelStyle={styles.buttonLabel}
            >
              Çalışma Başlat
            </Button>
          </Link>
          
          <Link href="/(app)/fuel-add" asChild>
            <Button 
              mode="contained-tonal" 
              style={styles.button}
              contentStyle={styles.buttonContent}
              labelStyle={styles.buttonLabel}
            >
              Yakıt Ekle
            </Button>
          </Link>
        </View>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  content: {
    flex: 1,
    paddingHorizontal: 24,
    paddingTop: 32,
  },
  userSection: {
    alignItems: 'center',
    marginBottom: 48,
  },
  welcomeText: {
    fontSize: 18,
    fontWeight: '600',
    color: '#333',
    marginTop: 16,
  },
  buttonContainer: {
    flex: 1,
  },
  button: {
    marginBottom: 16,
    borderRadius: 12,
    elevation: 2,
  },
  buttonContent: {
    height: 56,
  },
  buttonLabel: {
    fontSize: 16,
    fontWeight: '600',
  },
});