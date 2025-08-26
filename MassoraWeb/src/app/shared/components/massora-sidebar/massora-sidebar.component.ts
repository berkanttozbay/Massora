import { Component, OnInit } from '@angular/core';
// Oluşturduğumuz MenuItem interface'ini de import edelim
import { SIDEBAR_MENU_ITEMS, MenuItem } from '../../helpers/sidebar.constants';
import { AuthService } from '../../../auth.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './massora-sidebar.component.html',
  styleUrls: ['./massora-sidebar.component.css'],
  standalone: false
})
export class SidebarComponent implements OnInit {
  // filteredMenu'nün tipini MenuItem dizisi olarak belirtiyoruz
  public filteredMenu: MenuItem[] = [];
  public menuOpenState: { [key: string]: boolean } = {};

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    console.log("➡️ SidebarComponent OnInit tetiklendi.");

    this.authService.userData$.subscribe(userData => {
      console.log("👤 AuthService'ten gelen userData:", userData);

      const userRoleData = userData?.name;
      const userRoles: string[] = userRoleData
        ? (Array.isArray(userRoleData) ? userRoleData : [userRoleData])
        : [];

      console.log("🔑 Kullanıcının rolleri (userRoles):", userRoles);
      console.log("📋 Filtrelenecek ana menü (SIDEBAR_MENU_ITEMS):", SIDEBAR_MENU_ITEMS);

      this.filteredMenu = SIDEBAR_MENU_ITEMS.map(menuItem => {
        // Her bir ana menü öğesi için konsolda bir grup oluşturarak logları düzenli tutalım.
        console.group(`🔄 '${menuItem.title}' menü öğesi işleniyor...`);

        // 1. Alt menüleri (children) filtrele
        const filteredChildren = menuItem.children
          ? menuItem.children.filter((child: MenuItem) => {
              const hasAccess = child.roles?.some((role: string) => userRoles.includes(role));
              console.log(`- Alt öğe '${child.title}' için erişim kontrolü: ${hasAccess ? '✅ Var' : '❌ Yok'}`);
              return hasAccess;
            })
          : null;

        console.log("👶 Filtrelenmiş alt menü (filteredChildren):", filteredChildren);

        // 2. Ana menü öğesinin kendisinin görünür olup olmadığını kontrol et
        const isParentVisible = menuItem.roles?.some((role: string) => userRoles.includes(role));
        console.log(`👨‍👩‍👧 Ana menü '${menuItem.title}' için görünürlük (isParentVisible): ${isParentVisible ? '✅ Evet' : '❌ Hayır'}`);

        // 3. Karar verme: Ana menü görünürse VEYA gösterilecek alt menüsü varsa, menüye ekle
        if (isParentVisible || (filteredChildren && filteredChildren.length > 0)) {
          console.log(`👍 Karar: '${menuItem.title}' menüye EKLENECEK.`);
          console.groupEnd(); // Grubu kapat
          return {
            ...menuItem,
            children: filteredChildren,
          };
        }

        console.log(`👎 Karar: '${menuItem.title}' menüden KALDIRILACAK.`);
        console.groupEnd(); // Grubu kapat
        return null;
      }).filter((item): item is MenuItem => item !== null);

      console.log("✅🏁 Filtreleme sonrası son menü (this.filteredMenu):", this.filteredMenu);
    });
  }

  toggleMenu(title: string): void {
    this.menuOpenState[title] = !this.menuOpenState[title];
  }
}