export class UploadFileConfig {
    /** Multi/Single Selection */
    IsMultiFile: boolean = true; // True - Chọn nhiều, False - chọn một

    /** Edit or View Mode */
    Display: number = 0; // 0: giao diện sửa/cập nhật, 1: giao diện thêm mới, 2: giao diện xem chi tiết

    /** Accept Type */
    AcceptType: string[] = []; // Mảng chứa các định dạng file cho phép

    /** File Size */
    IsLimitMaxFileSize: boolean = false; // True - Giới hạn kích thước file, False - không giới hạn
    MaxFileSize: number = 0; // Giới hạn kích thước file (MB)

    /** File Quantity */
    IsLimitMaxFileQuantity: boolean = false; // True - Giới hạn số lượng file, False - không giới hạn

    /** Folder Upload */
    FolderUpload: string = ''; // Thư mục chứa file trên server

    /** Expand Expansion Panel */
    ExpandPanel: "Server" | "Local" = "Local"; // Mở rộng panel Server hay Local

    /** Title Component */
    Title: string = 'Tải lên tệp';

    /** Can Undo Delete */
    DeleteUndo: boolean = false; // True - Có thể hoàn tác khi xóa, False - không thể hoàn tác

     /** Upload type */
    TypeUpload: number = 0; // 1 - Upload file bình thường, 2 - Upload file từ kho lưu trữ
}

//Model file đã upload lên server
export class FileServerModel {
    Id: string = '';
    FileName: string = '';
    PreviewUrl:string='';
    Url: string = '';
    Description: string = '';
    FileSize: number = 0;
    Status: string = 'Đã tải lên';
    LocalDelete: boolean = false;
    IsDelete: boolean = false;// File có thể được xóa hay không
    TypeUpload: number = 0;
}

//Model file chưa upload lên server
export class FileLocalModel {
    Id: string = '';
    FileName: string = '';
    File: any = null;
    FileSize: number = 0;
    ContentTypes: string = '';
    PreviewUrl: string = '';
    Status: string = 'Chưa tải lên';
    LocalDelete: boolean = false;
    TypeUpload: number = 0;
}

//Model Icon
export class IconModel {
    MineType: string = '';
    IconName: string = '';
}

//Model lịch sử thêm, xoá
export class HistoryModel {
    Index: number = 0;
    FileIds: string[] = [];
    FileLocation: 'Server' | 'Local' = 'Local';
    Action: 'Add' | 'Delete' = 'Add';
}

//Model trả về cho component cha
export class UploadFileResult {
    FileServerList: FileServerModel[] = [];
    FileLocalList: FileLocalModel[] = [];
    History: HistoryModel[] = [];
}