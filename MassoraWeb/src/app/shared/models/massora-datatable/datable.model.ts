export class DatatableModel {
  public tabIcon: string = "";
  public tabTitle: string = "";
  public searchFilter: any;
  public filterIds?: (number | null)[] | null;
  public count?: number = 0;
  public filterId?: number | null;

  constructor(obj?: Partial<DatatableModel>) {
    if (obj)
      Object.assign(this, obj);
  }
}
