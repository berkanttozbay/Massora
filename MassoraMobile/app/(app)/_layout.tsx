import React from 'react';
import { Stack } from 'expo-router';

export default function AppLayout() {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="home" />
      <Stack.Screen name="work-tracking" />
      <Stack.Screen name="fuel-add" />
    </Stack>
  );
}