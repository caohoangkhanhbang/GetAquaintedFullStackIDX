import { BaseModel } from "src/app/_metronic/core/models/_base.model";

export class BacDaoTaoModel extends BaseModel {
  RowId: number;
  Code: string;
  Title: string;
  TenTiengAnh: string;
  NoiDung: string;
  GhiChu: string;
  HinhThucDaoTao: number;
  SoThuTu: number;

  clear() {
      this.RowId = 0;
      this.Code = '';
      this.Title = '';
      this.TenTiengAnh = '';
      this.NoiDung = '';
      this.GhiChu = '';
      this.HinhThucDaoTao = 0;
      this.SoThuTu = 0;
  }
}
