import { BaseModel } from "src/app/_metronic/core/models/_base.model";

export class TutorialModel extends BaseModel {
	RowID: number;
	EventCode: string;
	EventName: string;
	EventDate: string;
	EventSelect: string;
	clear() {
		this.RowID = 0;
		this.EventCode = '';
		this.EventName = '';
		this.EventDate = '';
		this.EventSelect = '';
	}
}
