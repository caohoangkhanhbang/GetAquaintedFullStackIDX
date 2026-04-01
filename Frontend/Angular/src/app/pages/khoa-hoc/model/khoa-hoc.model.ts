import { BaseModel } from "src/app/_metronic/core/models/_base.model";

export class KhoaHocModel extends BaseModel {
    id: number;
    TenKhoaHoc: string;
    NamHoc: number | string;
    CachViet: string;
    Disable: boolean;
    CreatedBy: string;
    CreatedDate: Date | string;
    TenNamHoc: string;

    clear() {
        this.id = 0;
        this.TenKhoaHoc = '';
        this.NamHoc = 0;
        this.CachViet = "";
        this.Disable = true;
        this.CreatedBy = "";
        this.CreatedDate = new Date();
        this.TenNamHoc = "";
    }
}
