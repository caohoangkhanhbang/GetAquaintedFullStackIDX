import { BaseModel } from "src/app/_metronic/core/models/_base.model";

export class LoaiDaoTaoModel extends BaseModel {
    id: number;
    MaLoaiDT: string;
    TenLoaiDT: string;
    TenTiengAnh: string;
    NoiDung: string;
    SoThuTu: number;
    GhiChu: string;
    IsDel: boolean;
    NguoiTao: string;
    NgayTao: Date | string;

    clear() {
        this.id = 0;
        this.MaLoaiDT = '';
        this.TenLoaiDT = '';
        this.TenTiengAnh = '';
        this.NoiDung = '';
        this.SoThuTu = 0;
        this.GhiChu = '';
        this.IsDel = false;
        this.NguoiTao = '';
        this.NgayTao = new Date();
    }
}
