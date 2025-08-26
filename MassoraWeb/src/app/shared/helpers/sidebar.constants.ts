// Menü elemanının yapısını tanımlayan arayüz (interface)
export interface MenuItem {
  title: string;
  icon: string;
  link: string;
  // children, MenuItem dizisi veya null olabilir
  children: MenuItem[] | null; 
  roles: string[];
  isSpecial?: boolean; // Bu özelliği de ekleyelim
}

// Sabit diziyi oluşturduğumuz interface ile tiplendirelim
export const SIDEBAR_MENU_ITEMS: MenuItem[] = [
    {
      title: 'Dashboard',
      icon: "",
      link: '/order/live-tracking',
      children: null,
      roles: ["Multillo.Transporter", "Multillo.Supplier"]
    },
    
    {
      title: 'Vehicles',
      icon: "assets/images/sidebar/truck.png",
      link: '/vehicle-getall',
      children: null,
      roles: ["admin", "driver"]
    },
    {
      title: 'Drivers',
      icon: "assets/images/sidebar/driver.png",
      link: '/driver-getall',
      children: null,
      roles: ["admin", "driver"]
    },
    {
      title: 'PartnerCompanies',
      icon: "assets/images/sidebar/company.png",
      link: '/partnerCompanies-getAll',
      children: null,
      roles: ["admin", "driver"]
    },
    {
      title: 'WorkHistories',
      icon: "assets/images/sidebar/work.png",
      link: '/workhistories',
      children: null,
      roles: ["admin", "driver"]
    },
    {
      title: 'FuelHistories',
      icon: "assets/images/sidebar/fuel.png",
      link: '/fuelhistories',
      children: null,
      roles: ["admin", "driver"]
    },
];