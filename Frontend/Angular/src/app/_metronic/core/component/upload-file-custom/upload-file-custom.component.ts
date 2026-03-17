import { ChangeDetectorRef, Component, EventEmitter, inject, Input, OnChanges, OnInit, Output, Renderer2, ViewChild } from "@angular/core";
import { FileLocalModel, FileServerModel, HistoryModel, IconModel, UploadFileConfig } from "./models/upload-file-custom.model";
import { environment } from "src/environments/environment";
import { LayoutUtilsService } from "../../utils/layout-utils.service";

@Component({
  selector: "app-upload-file-custom",
  templateUrl: "./upload-file-custom.component.html",
  styleUrls: ["./upload-file-custom.component.scss"],
})
export class UploadFileCustomComponent implements OnInit, OnChanges {
  private layoutUtilsService = inject(LayoutUtilsService);
  private changeDetect = inject(ChangeDetectorRef);

  @ViewChild("fileDropRef") fileDropRef: any;

  /** Config của Component */
  @Input() Config: UploadFileConfig = new UploadFileConfig();

  /** Các File đã upload lên server */
  @Input() FileServerList: FileServerModel[] = [];

  /** Các File chưa upload lên server */
  @Input() FileLocalListInput: FileLocalModel[] = [];

  /** List các file cần upload */
  @Output() ListFilesUpload = new EventEmitter<FileLocalModel[]>();

  /** List các file đã bị xoá - chỉ ghi lại các file đã upload bị xoá */
  @Output() ListFilesDelete = new EventEmitter<FileServerModel[]>();

  @Output() ListFileTuKhoLuuTru = new EventEmitter<any>();

  /** Các File mới thêm còn ở Local */
  FileLocalList: FileLocalModel[] = [];

  /** Các File Server đã bị xoá*/
  FileServerListDelete: FileServerModel[] = [];

  /** Lịch sử thêm, xoá */
  History: HistoryModel[] = [];

  /** Lịch sử Undo */
  Undo: HistoryModel[] = [];

  /** Lịch sử Redo */
  Redo: HistoryModel[] = [];

  /** Icon File */
  IconFile: IconModel[] = [
    {
      MineType: "xlsx",
      IconName: "far fa-file-excel",
    },
    {
      MineType: "xls",
      IconName: "far fa-file-excel",
    },
    {
      MineType: "docx",
      IconName: "far fa-file-word",
    },
    {
      MineType: "doc",
      IconName: "far fa-file-word",
    },
    {
      MineType: "pptx",
      IconName: "far fa-file-powerpoint",
    },
    {
      MineType: "ppt",
      IconName: "far fa-file-powerpoint",
    },
    {
      MineType: "pdf",
      IconName: "far fa-file-pdf",
    },
  ];

  /** Image Mine */
  ImageMine: string[] = [
    "jpeg",
    "png",
    "gif",
    "jpg",
    "svg",
    "bmp",
    "webp",
    "ico",
    "tif",
    "tiff",
    "jfif",
    "pjpeg",
    "pjp",
    "avif",
    "apng",
  ];

  FileServerListToShow: FileServerModel[] = [];

  typeUpload: number = 1; // =1 upload bình thường, 2 lấy file từ kho lưu trữ
  // Chọn file từ kho lưu trữ
  datasourceTaiLieu: any[] = [];
  displayedColumns: string[] = ["check", "FileName"];
  domain = environment.HOST_TUTORIAL_API;
  DSFile: any[] = [];
  DSFileLoad: any[] = [];
  constructor(

  ) { }

  ngOnInit(): void {
    this.FileServerList.forEach((f) => {
      f.PreviewUrl = this.funcdinhdang(f.FileName);
    });
    this.FileServerListToShow = this.FileServerList;
    // this.GetDataTaiLieuLuuTru();
  }

  ngOnChanges(changes: any): void {
    if (changes.Config) {
      this.Config = changes.Config.currentValue;
      if (this.Config.Display == 2 || this.Config.Display == 0) {
        this.Config.ExpandPanel = "Server";
      } else {
        this.Config.ExpandPanel = "Local";
      }
    }

    if (changes.FileLocalListInput) {
      this.FileLocalList = changes.FileLocalListInput.currentValue;
    }

    if (changes.FileServerList) {
      this.FileServerList = changes.FileServerList.currentValue;
      this.FileServerListToShow = this.FileServerList.filter(
        (x) => !x.LocalDelete
      );
    }
  }

  /** EmitEvent */
  emitEvent() {
    let listAdd = this.FileLocalList.filter((x) => x.Status == "Chưa tải lên");
    this.ListFilesUpload.emit(listAdd);

    let listDelete = this.FileServerList.filter((x) => x.LocalDelete == true);
    this.ListFilesDelete.emit(listDelete);
  }

  /** Mở rộng panel Server hay Local */
  handlePanel() {
    this.Config.ExpandPanel = this.Config.ExpandPanel == "Local" ? "Server" : "Local";
  }

  /** Lấy Icon
   * @param fileUrl (Đường dẫn file)
   */
  getIcon(fileUrl: string) {
    try {
      const extension: any = fileUrl.split(".").pop();
      const icon = this.IconFile.find((x) => x.MineType === extension);
      if (icon) {
        return icon.IconName;
      } else if (this.ImageMine.includes(extension)) {
        return "image";
      } else {
        return "far fa-file";
      }
    } catch {
      return "far fa-file";
    }
  }

  /**
   * Convert Files list to normal array list
   * @param files (Files List)
   */
  pushFilesToList(files: Array<any>) {
    if (this.Config.IsMultiFile) {
      let ListIdAdd: string[] = [];
      for (const item of files) {
        const file = item as File;
        const fileLocal = new FileLocalModel();
        fileLocal.Id = this.FileLocalList.length.toString();
        fileLocal.FileName = file.name;
        fileLocal.File = file;
        fileLocal.FileSize = file.size;
        fileLocal.ContentTypes = file.type;
        fileLocal.Status = "Chưa tải lên";

        if (fileLocal.ContentTypes.includes("image")) {
          fileLocal.PreviewUrl = URL.createObjectURL(file);
        } else {
          fileLocal.PreviewUrl = this.funcdinhdang(file.name);
        }
        this.FileLocalList.push(fileLocal);

        ListIdAdd.push(fileLocal.Id);
      }
      this.emitEvent();
      //Ghi lại lịch sử
      if (ListIdAdd.length > 0) {
        this.addHistory(ListIdAdd, "Local", "Add");
      }
    } else {
      if (files.length > 1) {
        this.layoutUtilsService.showError(
          "Vui lòng chọn 1 file duy nhất",
        );
        return;
      }
      //Trường hợp chọn 1 file duy nhất thì sẽ set danh sách file chọn từ kho lại bằng rỗng
      this.DSFileLoad = [];
      this.ListFileTuKhoLuuTru.emit(this.DSFileLoad);
      this.datasourceTaiLieu.forEach(function (item) {
        item.IsCheck = false;
      });
      let ListIdAdd: string[] = [];
      for (const item of files) {
        this.FileLocalList = [];
        const file = item as File;
        const fileLocal = new FileLocalModel();
        fileLocal.Id = this.FileLocalList.length.toString();
        fileLocal.FileName = file.name;
        fileLocal.File = file;
        fileLocal.FileSize = file.size;
        fileLocal.ContentTypes = file.type;
        fileLocal.Status = "Chưa tải lên";

        if (fileLocal.ContentTypes.includes("image")) {
          fileLocal.PreviewUrl = URL.createObjectURL(file);
        } else {
          fileLocal.PreviewUrl = this.funcdinhdang(file.name);
        }
        this.FileLocalList.push(fileLocal);

        ListIdAdd.push(fileLocal.Id);
      }
      this.emitEvent();
      //Ghi lại lịch sử
      if (ListIdAdd.length > 0) {
        this.addHistory(ListIdAdd, "Local", "Add");
      }
    }
  }

  /**
   * on file drop handler
   * @param $event (Files từ Drag & Drop)
   */
  onFileDropped($event: any) {
    this.pushFilesToList($event);
  }

  /**
   * handle file from browsing
   * @param files (Files từ nút chọn)
   */
  fileBrowseHandler(files: any) {
    this.pushFilesToList(files.target.files);
  }

  /** handel file from storage */
  onChooseFromStorage() {
    this.typeUpload = 2;
  }

  /**
   * Delete file from files list
   * @param index (Index của file cần xoá)
   */
  deleteFileLocal(id: string) {
    if (this.fileDropRef != undefined)
      this.fileDropRef.nativeElement.value = "";
    // this.FileLocalList.splice(index, 1);

    let index = this.FileLocalList.findIndex((x) => x.Id == id);
    if (index != -1) {
      //Ghi lại lịch sử
      // this.addHistory([this.FileLocalList[index].Id], 'Local', 'Delete');

      //Cập nhật status
      this.FileLocalList[index].LocalDelete = true;
      this.FileLocalList[index].Status = "Đã xoá";

      //Xoá file
      if (this.Config.DeleteUndo == false) {
        this.FileLocalList.splice(index, 1);
      }
    }
    this.emitEvent();
  }

  /** Undo Delete File Local
   * @param index (Index của file cần Undo)
   */
  undoDeleteFileLocal(id: string) {
    let index = this.FileLocalList.findIndex((x) => x.Id == id);
    if (index != -1) {
      //Ghi lại lịch sử
      // this.addHistory([this.FileLocalList[index].Id], 'Local', 'Add');

      //Cập nhật status
      this.FileLocalList[index].LocalDelete = false;
      this.FileLocalList[index].Status = "Chưa tải lên";
    }
  }

  /**
   * Delete file from files list
   * @param index (Index của file cần xoá)
   */
  deleteFileServer(id: string) {
    // this.FileServerList.splice(index, 1);
    // this.ListFilesDelete.emit(this.FileServerList);

    let index = this.FileServerList.findIndex((x) => x.Id == id);
    if (index != -1) {
      //Ghi lại lịch sử
      // this.addHistory([this.FileServerList[index].Id], 'Server', 'Delete');

      //Cập nhật status
      this.FileServerList[index].LocalDelete = true;
      this.FileServerList[index].Status = "Đã xoá";

      /*ĐÓNG LẠI VÌ CHỈ CẦN CẬP NHẬT TRẠNG THÁI DELETE, VẪN GIỮ VỀ MẶT SỐ LƯỢNG DATA*/
      // //Xoá file
      // if (this.Config.DeleteUndo == false) {
      //     this.FileServerList.splice(index, 1);
      // }
    }
    this.FileServerListToShow = this.FileServerList.filter(
      (x) => !x.LocalDelete
    );
    this.emitEvent();
  }

  /** Undo Delete File Server
   * @param index (Index của file cần Undo)
   */
  undoDeleteFileServer(id: string) {
    let index = this.FileServerList.findIndex((x) => x.Id == id);
    if (index != -1) {
      //Ghi lại lịch sử
      // this.addHistory([this.FileServerList[index].Id], 'Server', 'Add');

      //Cập nhật status
      this.FileServerList[index].LocalDelete = false;
      this.FileServerList[index].Status = "Đã tải lên";
    }
    this.FileServerListToShow = this.FileServerList.filter(
      (x) => !x.LocalDelete
    );
  }

  /**
   * format bytes
   * @param bytes (File size in bytes)
   * @param decimals (Decimals point)
   */
  formatBytes(bytes: any, decimals: any) {
    if (bytes === 0) {
      return "0 Bytes";
    }
    const k = 1024;
    const dm = decimals <= 0 ? 0 : decimals || 2;
    const sizes = ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + " " + sizes[i];
  }

  /**
   * Trả kết quả về cho component cha
   */
  returnResult() {
    //Trả về list file đã upload
    this.ListFilesUpload.emit(this.FileLocalList);

    //Trả về list file đã xoá
    let listDelete = this.FileServerList.filter((x) => x.LocalDelete == true);
    this.ListFilesDelete.emit(listDelete);
  }

  /**Thao tác Undo - Redo: Chưa dùng đến */
  /**
   * Thêm lịch sử
   * @param fileIds (Danh sách Id của các file)
   * @param fileLocation (Vị trí file: Local/Server)
   * @param action (Hành động: Add/Delete)
   */
  addHistory(
    fileIds: string[],
    fileLocation: "Server" | "Local",
    action: "Add" | "Delete"
  ) {
    let history = new HistoryModel();
    history.Index = this.History.length;
    history.FileIds = fileIds;
    history.FileLocation = fileLocation;
    history.Action = action;
    this.History.push(history);
  }

  /**
   * Undo (Hiện chỉ hổ trợ Undo xoá)
   */
  undo() {
    if (this.History.length > 0) {
      let lastHistory: any = this.History.pop();
      this.Undo.push(lastHistory);

      if (lastHistory.Action == "Delete") {
        if (lastHistory.FileLocation == "Local") {
          for (const id of lastHistory.FileIds) {
            let index = this.FileLocalList.findIndex((x) => x.Id == id);
            if (index != -1) {
              this.FileLocalList[index].LocalDelete = false;
              this.FileLocalList[index].Status = "Chưa tải lên";
            }
          }
        } else {
          for (const id of lastHistory.FileIds) {
            let index = this.FileServerList.findIndex((x) => x.Id == id);
            if (index != -1) {
              this.FileServerList[index].LocalDelete = false;
              this.FileServerList[index].Status = "Đã tải lên";
            }
          }
          this.FileServerListToShow = this.FileServerList.filter(
            (x) => !x.LocalDelete
          );
        }
      }
    }
  }

  /**
   * Redo (Hiện chỉ hổ trợ Undo xoá)
   */
  redo() {
    if (this.Undo.length > 0) {
      let lastUndo: any = this.Undo.pop();
      this.History.push(lastUndo);

      if (lastUndo.Action == "Delete") {
        if (lastUndo.FileLocation == "Local") {
          for (const id of lastUndo.FileIds) {
            let index = this.FileLocalList.findIndex((x) => x.Id == id);
            if (index != -1) {
              this.FileLocalList[index].LocalDelete = true;
              this.FileLocalList[index].Status = "Đã xoá";
            }
          }
        } else {
          for (const id of lastUndo.FileIds) {
            let index = this.FileServerList.findIndex((x) => x.Id == id);
            if (index != -1) {
              this.FileServerList[index].LocalDelete = true;
              this.FileServerList[index].Status = "Đã xoá";
            }
          }
          this.FileServerListToShow = this.FileServerList.filter(
            (x) => !x.LocalDelete
          );
        }
      }
    }
  }

  funcdinhdang(item: any) {
    if (!item)
      return "/assets/media/svg/files/folder.svg";
    var extn = item.split(".").pop();
    if (extn == "pdf" || extn == "PDF")
      return "/assets/media/svg/files/pdf.svg";
    else if (extn == "doc" || extn == "docx")
      return "/assets/media/svg/files/word.svg";
    else if (extn == "txt") return "/assets/media/svg/files/txt.svg";
    else if (extn == "xlsx" || extn == "xlsm")
      return "/assets/media/svg/files/excel.svg";
    else if (extn == "jpg" || extn == "jpeg" || extn == "gif" || extn == "png")
      return "/assets/media/svg/files/image.svg";
    else if (
      extn == "mp4" ||
      extn == "mov" ||
      extn == "avi" ||
      extn == "gif" ||
      extn == "mpeg" ||
      extn == "flv" ||
      extn == "wmv" ||
      extn == "divx" ||
      extn == "mkv" ||
      extn == "rmvb" ||
      extn == "dvd" ||
      extn == "3gp" ||
      extn == "webm"
    )
      return "/assets/media/svg/files/video.svg";
    return "/assets/media/svg/files/folder.svg";
  }

  //Xử lý chọn nhiều file từ kho lưu trữ
  GetDataTaiLieuLuuTru() {
    // this.QLTaiLieuLuuTruService.GetListAllTaiLieuLuuTru().subscribe((res: any) => {
    //   if (res) {
    //     this.datasourceTaiLieu = !res.data || res.data.length == 0 ? [] : res.data;
    //   }
    //   this.changeDetect.detectChanges();
    // });
  }

  checkValue(item: any) {
    item.IsCheck = !item.IsCheck;
    const file: any = {
      filename: item.FileName,
      id: item.Id,
      link: item.Link,
      isCheck: item.IsCheck,
    };
    const index = this.DSFileLoad.findIndex((x) => x.id === item.Id);
    if (index !== -1) {
      this.DSFileLoad[index].isCheck = false;
    } else if (item.IsCheck) {
      this.DSFile.push(file);
      // this.FileLocalList = [];
      // this.FileServerListToShow = [];
      // this.ListFilesUpload.emit(this.FileLocalList);
    }

    this.DSFileLoad = this.DSFile.filter((z) => z.isCheck);
    this.ListFileTuKhoLuuTru.emit(this.DSFileLoad);
  }

  //Xử lý dành cho chọn 1 file từ kho lưu trữ cá nhân
  removeClass(id: string, item: any) {
    this.datasourceTaiLieu.forEach((item) => {
      if (item.Id !== id) {
        item.IsCheck = false;
      } else item.IsCheck = true;
    });
    var element: any = document.getElementById(id);
    if (element.classList.contains("mat-radio-checked")) {
      element.classList.remove("mat-radio-checked");
      this.DSFileLoad = this.DSFileLoad.filter((file) => file.id !== id);
    } else {
      this.DSFileLoad = [];
      const file: any = {
        filename: item.FileName,
        id: item.Id,
        link: item.Link,
        isCheck: item.IsCheck,
      };
      this.FileLocalList = [];
      this.FileServerListToShow = [];
      this.ListFilesUpload.emit(this.FileLocalList);
      element.classList.add("mat-radio-checked");
      this.DSFileLoad.push(file);
    }
    this.ListFileTuKhoLuuTru.emit(this.DSFileLoad);
  }

  deleteFileTuKhoLuuTruLocal(id: string) {
    var element: any = document.getElementById(id);
    if (!this.Config.IsMultiFile) {
      element.classList.remove("mat-radio-checked");
    } else {
      const index = this.datasourceTaiLieu.findIndex((x) => x.Id === id);
      if (index !== -1) {
        this.datasourceTaiLieu[index].IsCheck = false;
      }
    }
    this.DSFile = this.DSFile.filter((z) => z.id !== id && z.isCheck);
    this.DSFileLoad = this.DSFile;
    this.ListFileTuKhoLuuTru.emit(this.DSFileLoad);
  }

  goBack() {
    this.typeUpload = 1;
  }

  // end  xử lý file từ kho lưu trữ
  GetLink(r: any) {
    return r.TypeUpload == 1 ? r.Url : this.domain + r.Url;
  }
}
