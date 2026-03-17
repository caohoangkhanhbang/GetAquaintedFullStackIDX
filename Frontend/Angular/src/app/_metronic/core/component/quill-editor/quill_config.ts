
import * as QuillNamespace from 'quill';
let Quill: any = QuillNamespace;
import { ImageHandler, Options } from 'ngx-quill-upload';
import QuillImageDropAndPaste from 'quill-image-drop-and-paste'
import { CookieService } from 'ngx-cookie-service';
import { environment } from 'src/environments/environment';
import BlotFormatter from 'quill-blot-formatter';


// Quill.register('modules/blotFormatter', BlotFormatter);
// Quill.register('modules/imageDropAndPaste', QuillImageDropAndPaste)
// Quill.register('modules/imageHandler', ImageHandler);
// Quill.register('modules/videoHandler', VideoHandler);


// var icons = Quill.import('ui/icons');
// icons['fullscreen'] = '<i class="fa fa-expand" style="color: #444"></i>';

const formats = [
    'background',
    'bold',
    'color',
    'font',
    'code',
    'italic',
    'link',
    'size',
    'strike',
    'script',
    'underline',
    'blockquote',
    'header',
    'indent',
    'list',
    'align',
    'direction',
    'code-block',
    'formula',
    // 'image',
    // 'video'
];

const quillConfig = {
    toolbar: {
        container: [
            ['bold', 'italic', 'underline'],   //, 'strike'     // toggled buttons
            [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
            ['code-block'],
            //   [{ 'header': 1 }, { 'header': 2 }],               // custom button values
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            [{ 'script': 'sub' }, { 'script': 'super' }],      // superscript/subscript
            [{ 'indent': '-1' }, { 'indent': '+1' }],          // outdent/indent
            //   [{ 'direction': 'rtl' }],                         // text direction

            // [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
            [{ 'header': [1, 2, 3, 4, 5, 6, false] }],

            //   [{ 'font':[]}],
            [{ 'align': [] }],

            //   ['clean'],                                         // remove formatting button

            ['link', 'image', 'video'],//, 'formula'

            ['fullscreen']

        ],
        handlers: {
            'fullscreen': customBoldHandler
        },
    },

    imageHandler: {
        upload: (file: any) => {
            return new Promise((resolve, reject) => {

                if (file.type === 'image/jpeg' || file.type === 'image/png' || file.type === 'image/jpg') { // File types supported for image
                    if (file.size < 10000000) { // Customize file size as per requirement

                        // Sample API Call
                        const uploadData = new FormData();
                        uploadData.append('file', file, file.name);

                        var xhr: any;
                        xhr = new XMLHttpRequest();
                        xhr.withCredentials = false;
                        xhr.open('POST', environment.HOST_JEEWORK_API_2023 + '/api/attachment/upload-img');
                        xhr.onload = function () {
                            var json;
                            if (xhr.status < 200 || xhr.status >= 300) {
                                reject('HTTP Error: ' + xhr.status);
                                return;
                            }
                            json = JSON.parse(xhr.responseText);
                            if (!json || typeof json.imageUrl != 'string') {
                                reject('Invalid JSON: ' + xhr.responseText);
                                return;
                            }
                            resolve(json.imageUrl);
                        };
                        xhr.send(uploadData);
                    } else {
                        reject('Size too large');
                        // Handle Image size large logic 
                    }
                } else {
                    reject('Unsupported type');
                    // Handle Unsupported type logic
                }
            });
        },
        accepts: ['png', 'jpg', 'jpeg', 'jfif'] // Extensions to allow for images (Optional) | Default - ['jpg', 'jpeg', 'png']
    } as Options,

    //   videoHandler: {
    //     upload: (file) => {
    //       return // your uploaded video URL as Promise<string>
    //     },
    //     accepts: ['mpeg', 'avi']  // Extensions to allow for videos (Optional) | Default - ['mp4', 'webm']
    //   } as Options,

    imageResize: true,
    // blotFormatter: {
    //     // empty object for default behaviour.
    //   },
    imageDropAndPaste: {
        // add an custom image handler
        handler: imageHandler
    }
};

let minHeight = '';
function customBoldHandler(this: any) {
    let container = this.quill.container;
    let myQuill = container.parentElement.parentElement;
    if (myQuill.style.position == 'fixed') {
        container.style.maxHeight = minHeight;
        container.style.minHeight = minHeight;
        myQuill.style.position = 'unset';
        myQuill.style.minHeight = 'unset';
        this.container.childNodes[8].firstChild.firstChild.className = 'fa fa-expand';
    }
    else {
        container.style.maxHeight = '100vh';
        minHeight = container.style.minHeight;
        container.style.minHeight = '100vh';
        myQuill.style.maxHeight = '100vh';
        myQuill.style.minHeight = '100vh';
        myQuill.style.backgroundColor = 'white';
        myQuill.style.maxWidth = '100vw';
        myQuill.style.position = 'fixed';
        myQuill.style.top = '0';
        myQuill.style.left = '0';
        myQuill.style.right = '0';
        this.container.childNodes[8].firstChild.firstChild.className = 'fa fa-compress';
    }
}

function imageHandler(this: any, imageDataUrl: any, type: any, imageData: any,) {
    const quill = this.quill;
    const blob = imageData.toBlob()
    const file = imageData.toFile()

    // generate a form data
    const formData = new FormData()

    // append blob data
    // formData.append('file', blob)

    // or just append the file
    formData.append('file', file, file.name);

    // upload image to your server
    var xhr: any;
    xhr = new XMLHttpRequest();
    xhr.withCredentials = false;
    xhr.open('POST', environment.HOST_JEEWORK_API_2023 + '/api/attachment/upload-img');
    // const access_token = JSON.parse(localStorage.getItem("getAuthFromLocalStorage")).access_token
    let access_token = '';
    xhr.setRequestHeader('Authorization', `Bearer ${access_token}`);
    xhr.onload = function () {
        var json;
        if (xhr.status < 200 || xhr.status >= 300) {
            return;
        }
        json = JSON.parse(xhr.responseText);
        if (!json || typeof json.imageUrl != 'string') {
            return;
        }

        let index = (quill.getSelection() || {}).index;
        if (index === undefined || index < 0) index = quill.getLength();
        quill.insertEmbed(index, 'image', json.imageUrl, 'user')
    };

    xhr.send(formData);

}

const editorStyles = {
    'border': '1px solid #ebedf2',
    'height': '400px',
    'font-size': '12pt'
};

const placeholder = "Nhập nội dung..."
export { quillConfig, editorStyles, placeholder, formats };