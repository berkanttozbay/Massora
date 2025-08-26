export class PaginationModel {
  public pageNumber: number = 1;
  public pageSize: number = 10;
  constructor(obj?: Partial<PaginationModel>) {
    if (obj)
      Object.assign(this, obj);
  }
}
