import { MatPaginator } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal, { SweetAlertOptions } from 'sweetalert2';


export enum MessageType {
    Create,
    Read,
    Update,
    Delete
}

@Injectable()
export class LayoutUtilsService {

    /**
     * Service constructor
     *
     * @param snackBar: MatSnackBar
     * @param dialog: MatDialog
     */

    /**
     * Showing (Mat-Snackbar) Notification
     *
     * @param message: string
     * @param type: MessageType
     * @param duration: number
     * @param showCloseButton: boolean
     * @param showUndoButton: boolean
     * @param undoButtonDuration: number
     * @param verticalPosition: 'top' | 'bottom' = 'top'
     */

    constructor(
    ) {
    }

    showSuccess(
        title: string
    ) {
        const successAlert: SweetAlertOptions = {
            icon: 'success',
            title: title,
            position: 'top',
            text: '',
            confirmButtonText: 'OK',
            timer: 4000,
            customClass: {
                popup: 'swl-custom-height',
                confirmButton: 'btn swl-confirm-btn',
            }
        };
        Swal.fire(successAlert);
    }

    showError(
        title: string
    ) {
        const successAlert: SweetAlertOptions = {
            icon: 'error',
            title: title,
            text: '',
            confirmButtonText: 'OK',
            timer: 999999999,
            customClass: {
                popup: 'swl-custom-height',
                confirmButton: 'btn swl-confirm-btn',
            }
        };
        Swal.fire(successAlert);
    }

    showWarning(
    ) {
        const successAlert: SweetAlertOptions = {
            icon: 'warning',
            title: 'Warring!',
            text: "Bạn có chắc chắn muốn đóng mà không lưu thay đổi?",
            showCancelButton: true,
            focusCancel: true,
            cancelButtonText: "Không",
            confirmButtonText: 'Có',
            customClass: {
                confirmButton: 'btn swl-confirm-btn',
                cancelButton: 'btn btn-active-light'
            }
        };
        return successAlert;
    }
}
