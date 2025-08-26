export class PaginationResultModel {
  public totalPages: number = 0;
  public totalCount: number = 0; // Toplam eleman sayısı
  public items: any[] = []// Şu anki sayfada gösterilecek veriler

  constructor(obj?: Partial<PaginationResultModel>) {
    if (obj)
      Object.assign(this, obj);
  }
}
