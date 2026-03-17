import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable()
export class TokenStorage {
	/**
	 * Get pageSize
	 * @returns {Observable<string>}
	 */
	public getPageSize(): Observable<string> {
		const size: string = "50";
		return of(size);
	}
}
