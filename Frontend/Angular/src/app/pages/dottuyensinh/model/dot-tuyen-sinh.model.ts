import { BaseModel } from "src/app/_metronic/core/models/_base.model";
export class DotTuyenSinhModel extends BaseModel {
    Id: number;
    NamHoc?: number | null;
    Dot?: number | null;
    TenDotTS?: string | null;
    KhoaHoc?: string | null;
    ThoiGianNhanHSTuNgay?: Date | string | null;
    ThoiGianNhanHSDenNgay?: Date | string | null;
    NgayInGBTT?: Date | string | null;
    ThoiGianLayHSTuNgay?: Date | string | null;
    ThoiGianLayHSDenNgay?: Date | string | null;
    NgayNhapHocDK?: Date | string | null;
    GhiChu?: string | null;
    NguoiTao?: string | null;
    NgayTao?: Date | string | null;
    Isdel: boolean;

    clear() {
        this.Id = 0;
        this.NamHoc = 0;
        this.Dot = 0;
        this.TenDotTS = '';
        this.KhoaHoc = '';
        this.ThoiGianNhanHSTuNgay = new Date();
        let date = new Date();
        date.setDate(date.getDate() + 7);
        this.ThoiGianNhanHSDenNgay = date;
        this.NgayInGBTT = new Date();
        this.ThoiGianLayHSTuNgay = new Date();
        this.ThoiGianLayHSDenNgay = date;
        this.NgayNhapHocDK = new Date();
        this.GhiChu = '';
        this.NguoiTao = '';
        this.NgayTao = new Date();
        this.Isdel = false;
    }

}