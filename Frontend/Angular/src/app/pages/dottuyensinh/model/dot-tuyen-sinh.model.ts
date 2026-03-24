import { BaseModel } from "src/app/_metronic/core/models/_base.model";
export class DotTuyenSinhModel extends BaseModel {
    Id: number;
    NamHoc?: number | null;
    Dot?: number | null;
    TenDotTS?: string | null;
    KhoaHoc?: number | null;
    ThoiGianNhanHSTuNgay?: Date | string | null;
    ThoiGianNhanHSDenNgay?: Date | string | null;
    NgayInGBTT?: Date | string | null;
    ThoiGianLayHSTuNgay?: Date | string | null;
    ThoiGianLayHSDenNgay?: Date | string | null;
    NgayNhapHocDK?: Date | string | null;
    GhiChu?: string | null;
    CreatedBy?: string | null;
    CreatedDate?: Date | string | null;
    HienThi?: boolean;
    KichHoat?: boolean;
    TenKhoaHoc?: string;
    TenNamHoc?: string;

    clear() {
        this.Id = 0;
        this.NamHoc = 0;
        this.Dot = 0;
        this.TenDotTS = '';
        this.KhoaHoc = 0;
        this.ThoiGianNhanHSTuNgay = null;
        this.ThoiGianNhanHSDenNgay = null;
        this.NgayInGBTT = null;
        this.ThoiGianLayHSTuNgay = null;
        this.ThoiGianLayHSDenNgay = null;
        this.NgayNhapHocDK = null;
        this.GhiChu = '';
        this.CreatedBy = '';
        this.CreatedDate = null;
        this.HienThi = true;
        this.KichHoat = true;
        this.TenKhoaHoc = '';
        this.TenNamHoc = '';
    }

}
