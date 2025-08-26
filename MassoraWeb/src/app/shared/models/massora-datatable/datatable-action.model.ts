export class DatatableActionModel {
  public id?: string;
  public name?: string;
  public description?: string;
  public status?: any;
  public icon?: string;
  public model?: any;
  public columns: ColumnListModel[] = [];
  public boxes: ColumnBoxModel[] = [];
  public avatar?: string;
}

export class ColumnListModel {
  public columnIcon?: string;
  public columnName?: string;
  public columnValue?: any;
}

export class ColumnBoxModel {
  public boxIcon?: string;
  public boxName?: string;
  public boxDate?: string;
  public boxHour?: string;
}