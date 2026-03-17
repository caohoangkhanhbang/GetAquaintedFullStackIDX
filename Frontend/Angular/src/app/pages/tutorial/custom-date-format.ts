import { MatDateFormats } from "@angular/material/core";

export const FORMATSDMY: MatDateFormats = {
	parse: {
		dateInput: 'DD/MM/YYYY'
	},
	display: {
		dateInput: 'DD/MM/YYYY',
		monthYearLabel: 'MMMM YYYY',
		dateA11yLabel: 'LL',
		monthYearA11yLabel: 'MMMM YYYY'
	}
};

export const FORMATSMY: MatDateFormats = {
	parse: {
		dateInput: 'MM/YYYY',
	},
	display: {
		dateInput: 'MM/YYYY',
		monthYearLabel: 'MMM YYYY',
		dateA11yLabel: 'LL',
		monthYearA11yLabel: 'MMMM YYYY',
	},
};


export const FORMATSY: MatDateFormats = {
	parse: {
		dateInput: 'YYYY',
	},
	display: {
		dateInput: 'YYYY',
		monthYearLabel: 'YYYY',
		dateA11yLabel: 'LL',
		monthYearA11yLabel: 'MMMM YYYY',
	},
};
