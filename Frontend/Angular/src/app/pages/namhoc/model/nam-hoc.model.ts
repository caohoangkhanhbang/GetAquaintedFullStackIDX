import { BaseModel } from "src/app/_metronic/core/models/_base.model";

export class NamHocModel extends BaseModel {
    id: number;
    NamHoc: number;
    NienHoc: string;
    CreatedBy: string;
    CreatedDate: Date | string;
    Disable: boolean;

    clear() {
        this.id = 0;
        this.NamHoc = 0;
        this.NienHoc = '';
        this.CreatedBy = '';
        this.Disable = true;
    }
}