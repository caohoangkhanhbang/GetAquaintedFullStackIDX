import { inject, Injectable } from "@angular/core";
import { MatPaginatorIntl } from "@angular/material/paginator";
import { TranslateService } from "@ngx-translate/core";

@Injectable()
export class MatPaginatorIntlCustom extends MatPaginatorIntl {
  private translate = inject(TranslateService);
  override nextPageLabel = this.translate.instant('COMMON.trangsau');
  override previousPageLabel = this.translate.instant('COMMON.trangtruoc');
  override firstPageLabel = this.translate.instant('COMMON.dautrang');
  override lastPageLabel = this.translate.instant('COMMON.cuoitrang');
  override itemsPerPageLabel = this.translate.instant('COMMON.somucmoitrang');

  // Tùy chỉnh văn bản "1 – 12 of 12"
  override getRangeLabel = (page: number, pageSize: number, length: number): string => {
	const startIndex = page * pageSize + 1;
	const endIndex = Math.min(startIndex + pageSize - 1, length);
	return this.translate.instant('COMMON.hienthicacdong') + ` ${startIndex} ` + this.translate.instant('COMMON.den') + ` ${endIndex} ` + this.translate.instant('COMMON.trong') + ` ${length}`;  // Bạn có thể thay đổi văn bản này theo ý muốn
  }
}