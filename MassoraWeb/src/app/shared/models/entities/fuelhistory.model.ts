export interface FuelHistoryModel {
  id: number;
  vehicleId: number;
  driverId : number ;
  fuelCompany : string;
  liter : number;
  fee : number;
  date: string;
  time : string;
  createdDate: Date;
}