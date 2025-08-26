import React, { useEffect, useState } from 'react';
import { View, Text, TextInput, Button, StyleSheet, Alert } from 'react-native';
import { Picker } from '@react-native-picker/picker';
import { apiService } from '../../services/api'; // api.ts dosyanızın yolunu doğru girdiğinizden emin olun

// Interface'leriniz
interface Vehicle {
  id: string;
  plate: string;
}

interface Driver {
  id: string;
  name: string;
}

const WorkHistoryScreen = () => {
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [drivers, setDrivers] = useState<Driver[]>([]);
  const [selectedVehicleId, setSelectedVehicleId] = useState('');
  const [selectedDriverId, setSelectedDriverId] = useState('');
  const [address, setAddress] = useState('');
  const [startTime, setStartTime] = useState<Date | null>(null);
  const [endTime, setEndTime] = useState<Date | null>(null);
  const [isRunning, setIsRunning] = useState(false);
  const [durationInMinutes, setDurationInMinutes] = useState<number | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      // 1. Fonksiyonun başladığını görelim
      console.log('[DEBUG] 1. fetchData fonksiyonu tetiklendi.');
  
      try {
        // 2. try bloğuna girildi mi?
        console.log('[DEBUG] 2. try bloğuna girildi. API istekleri hazırlanıyor...');
  
        // 3. Promise.all çağrılmadan hemen önceki an
        console.log('[DEBUG] 3. Promise.all ile apiService metodları çağrılacak...');
  
        const [vehiclesData, driversData] = await Promise.all([
          apiService.getVehicles(),
          apiService.getDrivers()
        ]);
  
        // 4. API'den yanıt geldi mi? (Eğer buraya ulaşıyorsa, istek gitmiş demektir)
        console.log('[DEBUG] 4. API istekleri başarıyla tamamlandı. Yanıtlar işleniyor.');
        console.log("Gelen Araç Verisi:", JSON.stringify(vehiclesData, null, 2));
        console.log("Gelen Sürücü Verisi:", JSON.stringify(driversData, null, 2));
  
        setVehicles(vehiclesData);
        setDrivers(driversData);
  
        // 5. State güncellendi mi?
        console.log('[DEBUG] 5. State (setVehicles, setDrivers) güncellendi.');
  
      } catch (error) {
        // !!! HATA YAKALANDI MI? BU EN ÖNEMLİ KISIM !!!
        console.error('----------------------------------------------------');
        console.error('[DEBUG] HATA: fetchData içindeki CATCH bloğuna düşüldü!');
        console.error('Hata Mesajı:', (error as Error).message);
        console.error('Tüm Hata Objesi:', error);
        console.error('----------------------------------------------------');
        Alert.alert('Hata', 'Veriler yüklenirken bir sorun oluştu. Lütfen konsolu kontrol edin.');
      }
    };
  
    fetchData();
  }, []);

  const handleStart = () => {
    setStartTime(new Date());
    setIsRunning(true);
  };

  const handleStop = () => {
    const now = new Date();
    setEndTime(now);
    setIsRunning(false);
    if (startTime) {
      const diff = (now.getTime() - startTime.getTime()) / (1000 * 60);
      setDurationInMinutes(Math.round(diff));
    }
  };

  const handleSubmit = async () => {
    if (!selectedVehicleId || !startTime || !endTime) {
      Alert.alert('Eksik Bilgi', 'Lütfen araç seçimi ile başlangıç ve bitiş zamanlarını doldurun.');
      return;
    }

    const workHistoryPayload = {
      vehicleId: selectedVehicleId,
      address: address,
      date: new Date().toISOString().split('T')[0],
      startTime: startTime.toISOString(),
      endTime: endTime.toISOString(),
      driverId: selectedDriverId || null,
      partnerCompanyId: null, // Bu alanları ihtiyaca göre doldurabilirsiniz
      calculatedDriverFee: 0,
      calculatedPartnerFee: 0
    };

    try {
      await apiService.addWorkHistory(workHistoryPayload);
      Alert.alert('Başarılı', 'Kayıt başarıyla eklendi!');
      
      // Formu temizle
      setStartTime(null);
      setEndTime(null);
      setAddress('');
      setSelectedVehicleId('');
      setSelectedDriverId('');
      setDurationInMinutes(null);

    } catch (error) {
      console.error('Ekleme hatası:', error);
      Alert.alert('Hata', 'Kayıt eklenirken bir sorun oluştu.');
    }
  };

  return (
    <View style={styles.container}>
      <Text>Araç Seç:</Text>
      <Picker
        selectedValue={selectedVehicleId}
        onValueChange={(itemValue) => setSelectedVehicleId(itemValue)}
      >
        <Picker.Item label="Seçiniz" value="" />
        {vehicles.map((vehicle) => (
          <Picker.Item key={vehicle.id} label={vehicle.plate} value={vehicle.id} />
        ))}
      </Picker>

      <Text>Sürücü Seç:</Text>
      <Picker
        selectedValue={selectedDriverId}
        onValueChange={(itemValue) => setSelectedDriverId(itemValue)}
      >
        <Picker.Item label="Seçiniz" value="" />
        {drivers.map((driver) => (
          <Picker.Item key={driver.id} label={driver.name} value={driver.id} />
        ))}
      </Picker>

      <Text>Adres:</Text>
      <TextInput value={address} onChangeText={setAddress} style={styles.input} />

      <View style={styles.buttonRow}>
        <Button title="Başlat" onPress={handleStart} disabled={isRunning} />
        <Button title="Bitir" onPress={handleStop} disabled={!isRunning} />
      </View>

      {startTime && <Text>Başlangıç: {startTime.toLocaleTimeString()}</Text>}
      {endTime && <Text>Bitiş: {endTime.toLocaleTimeString()}</Text>}
      {durationInMinutes !== null && <Text>Süre: {durationInMinutes} dk</Text>}

      <Button title="Kaydet" onPress={handleSubmit} disabled={!endTime} />
    </View>
  );
};

export default WorkHistoryScreen;

const styles = StyleSheet.create({
  container: {
    padding: 16,
  },
  input: {
    borderBottomWidth: 1,
    marginBottom: 12,
    padding: 8,
  },
  buttonRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginVertical: 12,
  },
});