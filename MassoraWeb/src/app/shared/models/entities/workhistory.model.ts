export interface WorkHistoryModel {
  id: number;
  driverId : number;
  vehicleId: number;
  companyId: number;
  partnerCompanyId: number;
  date : string;
  startTime: string;
  endTime: string;
  calculatedDriverFee: number;
  calculatedPartnerFee: number;
  address: string;
  createdDate: Date;
}