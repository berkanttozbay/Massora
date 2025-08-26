import React, { useState, useEffect } from 'react';
import { View, StyleSheet, Alert, ScrollView } from 'react-native';
import { Button, Text, Surface, SegmentedButtons, TextInput, ActivityIndicator } from 'react-native-paper';
import { SafeAreaView } from 'react-native-safe-area-context';
import { router } from 'expo-router';
import { apiService } from '../../services/api';

interface Vehicle {
  id: number;
  name: string;
  plate: string;
}

export default function FuelAddScreen() {
  const [selectedVehicle, setSelectedVehicle] = useState<string>('');
  const [liters, setLiters] = useState('');
  const [cost, setCost] = useState('');
  const [loading, setLoading] = useState(false);
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [vehiclesLoading, setVehiclesLoading] = useState(true);

  useEffect(() => {
    loadVehicles();
  }, []);

  const loadVehicles = async () => {
    try {
      setVehiclesLoading(true);
      const vehiclesData = await apiService.getVehicles();
      setVehicles(vehiclesData);
    } catch (error) {
      console.error('Error loading vehicles:', error);
      Alert.alert('Hata', 'Araç listesi yüklenemedi');
    } finally {
      setVehiclesLoading(false);
    }
  };

  const validateForm = (): boolean => {
    if (!selectedVehicle) {
      Alert.alert('Hata', 'Lütfen bir araç seçin');
      return false;
    }
    if (!liters || parseFloat(liters) <= 0) {
      Alert.alert('Hata', 'Lütfen geçerli bir yakıt miktarı girin');
      return false;
    }
    if (!cost || parseFloat(cost) <= 0) {
      Alert.alert('Hata', 'Lütfen geçerli bir tutar girin');
      return false;
    }
    return true;
  };

  const handleSave = async () => {
    if (!validateForm()) return;

    setLoading(true);
    try {
      await apiService.addFuel(parseInt(selectedVehicle), parseFloat(liters), parseFloat(cost));
      
      Alert.alert('Başarılı', 'Yakıt kaydı eklendi', [
        { text: 'Tamam', onPress: () => router.back() }
      ]);
    } catch (error) {
      console.error('Save fuel error:', error);
      Alert.alert('Hata', 'Yakıt kaydı eklenirken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const calculatePricePerLiter = (): string => {
    if (!liters || !cost) return '0.00';
    const pricePerLiter = parseFloat(cost) / parseFloat(liters);
    return pricePerLiter.toFixed(2);
  };

  if (vehiclesLoading) {
    return (
      <SafeAreaView style={styles.container}>
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color="#2196F3" />
          <Text style={styles.loadingText}>Araçlar yükleniyor...</Text>
        </View>
      </SafeAreaView>
    );
  }

  return (
    <SafeAreaView style={styles.container}>
      <ScrollView contentContainerStyle={styles.scrollContent}>
        <Text style={styles.title}>Yakıt Ekleme</Text>

        {/* Araç Seçimi */}
        <Surface style={styles.section} elevation={2}>
          <Text style={styles.sectionTitle}>Araç Seçin</Text>
          <SegmentedButtons
            value={selectedVehicle}
            onValueChange={setSelectedVehicle}
            buttons={vehicles.map(vehicle => ({
              value: vehicle.id.toString(),
              label: `${vehicle.name}\n${vehicle.plate}`,
            }))}
            style={styles.segmentedButtons}
          />
        </Surface>

        {/* Yakıt Bilgileri */}
        <Surface style={styles.section} elevation={2}>
          <Text style={styles.sectionTitle}>Yakıt Bilgileri</Text>
          
          <TextInput
            label="Alınan Yakıt (Litre)"
            value={liters}
            onChangeText={setLiters}
            keyboardType="numeric"
            mode="outlined"
            style={styles.input}
            right={<TextInput.Affix text="L" />}
          />

          <TextInput
            label="Toplam Tutar (₺)"
            value={cost}
            onChangeText={setCost}
            keyboardType="numeric"
            mode="outlined"
            style={styles.input}
            right={<TextInput.Affix text="₺" />}
          />

          {liters && cost && (
            <View style={styles.priceInfo}>
              <Text style={styles.priceLabel}>Litre Başına Fiyat:</Text>
              <Text style={styles.priceValue}>{calculatePricePerLiter()} ₺/L</Text>
            </View>
          )}
        </Surface>

        {/* Kaydet Butonu */}
        <View style={styles.buttonContainer}>
          <Button
            mode="contained"
            onPress={handleSave}
            loading={loading}
            disabled={loading || !selectedVehicle || !liters || !cost}
            style={styles.saveButton}
            contentStyle={styles.buttonContent}
            labelStyle={styles.buttonLabel}
          >
            KAYDET
          </Button>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  loadingText: {
    marginTop: 16,
    fontSize: 16,
    color: '#666',
  },
  scrollContent: {
    paddingHorizontal: 24,
    paddingTop: 24,
    paddingBottom: 24,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 24,
    textAlign: 'center',
  },
  section: {
    padding: 20,
    marginBottom: 24,
    borderRadius: 12,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: '600',
    color: '#333',
    marginBottom: 16,
  },
  segmentedButtons: {
    marginTop: 8,
  },
  input: {
    marginBottom: 16,
  },
  priceInfo: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 12,
    paddingHorizontal: 16,
    backgroundColor: '#f0f0f0',
    borderRadius: 8,
  },
  priceLabel: {
    fontSize: 16,
    color: '#666',
  },
  priceValue: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#2196F3',
  },
  buttonContainer: {
    marginTop: 24,
  },
  saveButton: {
    backgroundColor: '#2196F3',
    borderRadius: 12,
    elevation: 4,
  },
  buttonContent: {
    height: 56,
  },
  buttonLabel: {
    fontSize: 18,
    fontWeight: '600',
  },
});