/**
* http://github.com/valums/file-uploader
* 
* Multiple file upload component with progress-bar, drag-and-drop. 
* © 2010 Andrew Valums ( andrew(at)valums.com ) 
* 
* Licensed under GNU GPL 2 or later and GNU LGPL 2 or later, see license.txt.
*/

//
// Helper functions
//

var qq = qq || {};

/**
* Adds all missing properties from second obj to first obj
*/
qq.extend = function (first, second) {
    for (var prop in second) {
        first[prop] = second[prop];
    }
};

/**
* Searches for a given element in the array, returns -1 if it is not present.
* @param {Number} [from] The index at which to begin the search
*/
qq.indexOf = function (arr, elt, from) {
    if (arr.indexOf) return arr.indexOf(elt, from);

    from = from || 0;
    var len = arr.length;

    if (from < 0) from += len;

    for (; from < len; from++) {
        if (from in arr && arr[from] === elt) {
            return from;
        }
    }
    return -1;
};

qq.getUniqueId = (function () {
    var id = 0;
    return function () { return id++; };
})();

//
// Events

qq.attach = function (element, type, fn) {
    if (element.addEventListener) {
        element.addEventListener(type, fn, false);
    } else if (element.attachEvent) {
        element.attachEvent('on' + type, fn);
    }
};
qq.detach = function (element, type, fn) {
    if (element.removeEventListener) {
        element.removeEventListener(type, fn, false);
    } else if (element.attachEvent) {
        element.detachEvent('on' + type, fn);
    }
};

qq.preventDefault = function (e) {
    if (e.preventDefault) {
        e.preventDefault();
    } else {
        e.returnValue = false;
    }
};

//
// Node manipulations

/**
* Insert node a before node b.
*/
qq.insertBefore = function (a, b) {
    b.parentNode.insertBefore(a, b);
};
qq.remove = function (element) {
    element.parentNode.removeChild(element);
};

qq.contains = function (parent, descendant) {
    // compareposition returns false in this case
    if (parent == descendant) return true;

    if (parent.contains) {
        return parent.contains(descendant);
    } else {
        return !!(descendant.compareDocumentPosition(parent) & 8);
    }
};

/**
* Creates and returns element from html string
* Uses innerHTML to create an element
*/
qq.toElement = (function () {
    var div = document.createElement('div');
    return function (html) {
        div.innerHTML = html;
        var element = div.firstChild;
        div.removeChild(element);
        return element;
    };
})();

//
// Node properties and attributes

/**
* Sets styles for an element.
* Fixes opacity in IE6-8.
*/
qq.css = function (element, styles) {
    if (styles.opacity != null) {
        if (typeof element.style.opacity != 'string' && typeof (element.filters) != 'undefined') {
            styles.filter = 'alpha(opacity=' + Math.round(100 * styles.opacity) + ')';
        }
    }
    qq.extend(element.style, styles);
};
qq.hasClass = function (element, name) {
    var re = new RegExp('(^| )' + name + '( |$)');
    return re.test(element.className);
};
qq.addClass = function (element, name) {
    if (!qq.hasClass(element, name)) {
        element.className += ' ' + name;
    }
};
qq.removeClass = function (element, name) {
    var re = new RegExp('(^| )' + name + '( |$)');
    element.className = element.className.replace(re, ' ').replace(/^\s+|\s+$/g, "");
};
qq.setText = function (element, text) {
    element.innerText = text;
    element.textContent = text;
};

//
// Selecting elements

qq.children = function (element) {
    var children = [],
    child = element.firstChild;

    while (child) {
        if (child.nodeType == 1) {
            children.push(child);
        }
        child = child.nextSibling;
    }

    return children;
};

qq.getByClass = function (element, className) {
    if (element.querySelectorAll) {
        return element.querySelectorAll('.' + className);
    }

    var result = [];
    var candidates = element.getElementsByTagName("*");
    var len = candidates.length;

    for (var i = 0; i < len; i++) {
        if (qq.hasClass(candidates[i], className)) {
            result.push(candidates[i]);
        }
    }
    return result;
};

/**
* obj2url() takes a json-object as argument and generates
* a querystring. pretty much like jQuery.param()
* 
* how to use:
*
*    `qq.obj2url({a:'b',c:'d'},'http://any.url/upload?otherParam=value');`
*
* will result in:
*
*    `http://any.url/upload?otherParam=value&a=b&c=d`
*
* @param  Object JSON-Object
* @param  String current querystring-part
* @return String encoded querystring
*/
qq.obj2url = function (obj, temp, prefixDone) {
    var uristrings = [],
        prefix = '&',
        add = function (nextObj, i) {
            var nextTemp = temp
                ? (/\[\]$/.test(temp)) // prevent double-encoding
                   ? temp
                   : temp + '[' + i + ']'
                : i;
            if ((nextTemp != 'undefined') && (i != 'undefined')) {
                uristrings.push(
                    (typeof nextObj === 'object')
                        ? qq.obj2url(nextObj, nextTemp, true)
                        : (Object.prototype.toString.call(nextObj) === '[object Function]')
                            ? encodeURIComponent(nextTemp) + '=' + encodeURIComponent(nextObj())
                            : encodeURIComponent(nextTemp) + '=' + encodeURIComponent(nextObj)
                );
            }
        };

    if (!prefixDone && temp) {
        prefix = (/\?/.test(temp)) ? (/\?$/.test(temp)) ? '' : '&' : '?';
        uristrings.push(temp);
        uristrings.push(qq.obj2url(obj));
    } else if ((Object.prototype.toString.call(obj) === '[object Array]') && (typeof obj != 'undefined')) {
        // we wont use a for-in-loop on an array (performance)
        for (var i = 0, len = obj.length; i < len; ++i) {
            add(obj[i], i);
        }
    } else if ((typeof obj != 'undefined') && (obj !== null) && (typeof obj === "object")) {
        // for anything else but a scalar, we will use for-in-loop
        for (var i in obj) {
            add(obj[i], i);
        }
    } else {
        uristrings.push(encodeURIComponent(temp) + '=' + encodeURIComponent(obj));
    }

    return uristrings.join(prefix)
                     .replace(/^&/, '')
                     .replace(/%20/g, '+');
};

//
//
// Uploader Classes
//
//

var qq = qq || {};

/**
* Creates upload button, validates upload, but doesn't create file list or dd. 
*/
qq.FileUploaderBasic = function (o) {
    this._options = {
        // set to true to see the server response
        debug: false,
        action: '/server/upload',
        params: {},
        button: null,
        multiple: true,
        maxConnections: 3,
        maxFiles: 0,
        // validation        
        allowedExtensions: [ "doc", "docx", "msg", "xls", "xlsx", "csv", "txt", "rtf", "html", "zip", "rar", "mp3", "wma", "mpg", "flv", "avi", "jpg", "jpeg", "png", "gif", "ppt", "pptx", "txt","tif","tiff","bmp"],
        sizeLimit: 0,
        minSizeLimit: 0,
        duplicateCheck: true,
        // events
        // return false to cancel submit
        onSubmit: function (id, fileName) { },
        onProgress: function (id, fileName, loaded, total) { },
        onComplete: function (id, fileName, responseJSON) { },
        onCancel: function (id, fileName) { },
        // messages                
        messages: {
            typeError: "{file} has invalid extension. Only {extensions} are allowed.",
            sizeError: "{file} is too large, maximum file size is {sizeLimit}.",
            minSizeError: "{file} is too small, minimum file size is {minSizeLimit}.",
            maxFilesError: "You cannot upload more than {maxFiles} files.",
            emptyError: "{file} is empty, please select files again without it.",
            alreadyExistError: "{file} is already uploaded. You cannot upload file with same name.",
            onLeave: "The files are being uploaded, if you leave now the upload will be cancelled.",
            InvalidCharacter: "{file} filename is invalid, Please verify and try again.<ul> " +
                "<li>Filename should not contains (~ # % & * { } \ : < > ? / + | \" ; ..) character(s)</li>" +
                "<li>Filename should not start with period(.) character</li>" +
                "<li>Filename should not contain consecutive period(.) character</li>" +
                "<li>Filename should not end with period(.) character</li>" +
                "</ul>"
        },
        showMessage: function (message) {
            AlertModal(ResourceManager.Title.Error, message);
        }
    };
    qq.extend(this._options, o);

    // number of files being uploaded
    this._filesInProgress = 0;
    this._handler = this._createUploadHandler();

    if (this._options.button) {
        this._button = this._createUploadButton(this._options.button);
    }

    this._preventLeaveInProgress();
};

qq.FileUploaderBasic.prototype = {
    setParams: function (params) {
        this._options.params = params;
    },
    getInProgress: function () {
        return this._filesInProgress;
    },
    _createUploadButton: function (element) {
        var self = this;

        return new qq.UploadButton({
            element: element,
            multiple: this._options.multiple && qq.UploadHandlerXhr.isSupported(),
            onChange: function (input) {
                self._onInputChange(input);
            }
        });
    },
    _createUploadHandler: function () {
        var self = this,
            handlerClass;

        if (qq.UploadHandlerXhr.isSupported()) {
            handlerClass = 'UploadHandlerXhr';
        } else {
            handlerClass = 'UploadHandlerForm';
        }

        var handler = new qq[handlerClass]({
            debug: this._options.debug,
            action: this._options.action,
            maxConnections: this._options.maxConnections,
            onProgress: function (id, fileName, loaded, total) {
                self._onProgress(id, fileName, loaded, total);
                self._options.onProgress(id, fileName, loaded, total);
            },
            onComplete: function (id, fileName, result) {
                self._onComplete(id, fileName, result);
                self._options.onComplete(id, fileName, result);
            },
            onCancel: function (id, fileName) {
                self._onCancel(id, fileName);
                self._options.onCancel(id, fileName);
            }
        });

        return handler;
    },
    _preventLeaveInProgress: function () {
        var self = this;

        qq.attach(window, 'beforeunload', function (e) {
            if (!self._filesInProgress) { return; }

            var e = e || window.event;
            // for ie, ff
            e.returnValue = self._options.messages.onLeave;
            // for webkit
            return self._options.messages.onLeave;
        });
    },
    _onSubmit: function (id, fileName) {
        this._filesInProgress++;
    },
    _onProgress: function (id, fileName, loaded, total) {
    },
    _onComplete: function (id, fileName, result) {
        this._filesInProgress--;
        if (result.error) {
            this._options.showMessage(result.error);
        }
    },
    _onCancel: function (id, fileName) {
        this._filesInProgress--;
    },
    _onInputChange: function (input) {
        if (this._handler instanceof qq.UploadHandlerXhr) {
            this._uploadFileList(input.files);
        } else {
            if (this._validateFile(input)) {
                this._uploadFile(input);
            }
        }
        this._button.reset();
    },
    _uploadFileList: function (files) {
        for (var i = 0; i < files.length; i++) {
            if (!this._validateFile(files[i])) {
                return;
            }
        }

        for (var i = 0; i < files.length; i++) {
            this._uploadFile(files[i]);
        }
    },
    _uploadFile: function (fileContainer) {
        var id = this._handler.add(fileContainer);
        var fileName = this._handler.getName(id);

        if (this._options.onSubmit(id, fileName) !== false) {
            this._onSubmit(id, fileName);
            this._handler.upload(id, this._options.params);
        }
    },
    _validateFile: function (file) {
        var name, size;

        if (file.value) {
            // it is a file input            
            // get input value and remove path to normalize
            name = file.value.replace(/.*(\/|\\)/, "");
        } else {
            // fix missing properties in Safari
            name = file.fileName != null ? file.fileName : file.name;
            size = file.fileSize != null ? file.fileSize : file.size;
        }
        var fileList = [];
        if (this._options.duplicateCheck) {
            $("li.qq-upload-success").each(function () {
                fileList.push($.trim($(this).find(".qq-upload-file").text()));
            });
        }
        var match = (new RegExp('[,~#%\&{}+\|]|\\.\\.|^\\.|\\.$')).test(name);
        if (!this._isAllowedExtension(name)) {
            this._error('typeError', name);
            return false;

        } else if (match) {
            this._error('InvalidCharacter', name);
            return false;
        } else if (size === 0) {
            this._error('emptyError', name);
            return false;

        } else if (size && this._options.sizeLimit && size > this._options.sizeLimit) {
            this._error('sizeError', name);
            return false;

        } else if (size && size < this._options.minSizeLimit) {
            this._error('minSizeError', name);
            return false;
        }
        else if (this._options.maxFiles && $(this._listElement).find("li.qq-upload-success").length >= this._options.maxFiles) {
            this._error('maxFilesError', this._options.maxFiles);
            return false;
        }
        else if (this._options.duplicateCheck && $.inArray(name, fileList) >= 0) {
            this._error('alreadyExistError', name);
            return false;
        }
        return true;
    },
    _error: function (code, fileName) {
        var message = this._options.messages[code];
        function r(name, replacement) { message = message.replace(name, replacement); }

        r('{file}', this._formatFileName(fileName));
        r('{extensions}', this._options.allowedExtensions.join(', '));
        r('{sizeLimit}', this._formatSize(this._options.sizeLimit));
        r('{minSizeLimit}', this._formatSize(this._options.minSizeLimit));
        r('{maxFiles}', this._options.maxFiles);
        this._options.showMessage(message);
    },
    _formatFileName: function (name) {
        if (name.length > 33) {
            name = name.slice(0, 19) + '...' + name.slice(-13);
        }
        return name;
    },
    _isAllowedExtension: function (fileName) {
        var ext = (-1 !== fileName.indexOf('.')) ? fileName.replace(/.*[.]/, '').toLowerCase() : '';
        var allowed = this._options.allowedExtensions;
        
        var notAllowedExtension = ["dll", "exe", "bat", "3g2", "3gp", "3gp2", "3gpp", "3gpp2", "3mm", "3p2", "60d", "aaf", "aec", "aep", "aepx", "aet", "aetx", "ajp", "ale", "am", "amc", "amv", "amx", "anim", "anx", "aqt", "arcut", "arf", "asf", "asx", "avb", "avc", "avchd", "avd", "avi", "avp", "avs", "avs", "avv", "awlive", "axm", "axv", "bdm", "bdmv", "bdt2", "bdt3", "bik", "bin", "bix", "bmc", "bmk", "bnp", "box", "bs4", "bsf", "bu", "bvr", "byu", "camproj", "camrec", "camv", "ced", "cel", "cine", "cip", "clk", "clpi", "cmmp", "cmmtpl", "cmproj", "cmrec", "cmv", "cpi", "cpvc", "cst", "cvc", "cx3", "d2v", "d3v", "dash", "dat", "dav", "db2", "dce", "dck", "dcr", "dcr", "ddat", "dif", "dir", "divx", "dlx", "dmb", "dmsd", "dmsd3d", "dmsm", "dmsm3d", "dmss", "dmx", "dnc", "dpa", "dpg", "dream", "dsy", "dv", "dv-avi", "dv4", "dvdmedia", "dvr", "dvr-ms", "dvx", "dxr", "dzm", "dzp", "dzt", "edl", "evo", "evo", "exo", "eye", "eyetv", "ezt", "f4f", "f4p", "f4v", "fbr", "fbr", "fbz", "fcarch", "fcp", "fcproject", "ffd", "ffm", "flc", "flh", "fli", "flv", "flx", "fpdx", "ftc", "g64", "gcs", "gfp", "gifv", "gl", "gom", "grasp", "gts", "gvi", "gvp", "gxf", "h264", "hdmov", "hdv", "hkm", "ifo", "imovielibrary", "imoviemobile", "imovieproj", "imovieproject", "inp", "int", "ircp", "irf", "ism", "ismc", "ismclip", "ismv", "iva", "ivf", "ivr", "ivs", "izz", "izzy", "jmv", "jss", "jts", "jtv", "k3g", "kdenlive", "kmv", "ktn", "lrec", "lrv", "lsf", "lsx", "lvix", "m15", "m1pg", "m1v", "m21", "m21", "m2a", "m2p", "m2t", "m2ts", "m2v", "m4e", "m4u", "m4v", "m75", "mani", "meta", "mgv", "mj2", "mjp", "mjpeg", "mjpg", "mk3d", "mkv", "mmv", "mnv", "mob", "mod", "modd", "moff", "moi", "moov", "mov", "movie", "mp21", "mp21", "mp2v", "mp4", "mp4infovid", "mp4v", "mpe", "mpeg", "mpeg1", "mpeg2", "mpeg4", "mpf", "mpg", "mpg2", "mpgindex", "mpl", "mpl", "mpls", "mproj", "mpsub", "mpv", "mpv2", "mqv", "msdvd", "mse", "msh", "mswmm", "mt2s", "mts", "mtv", "mvb", "mvc", "mvd", "mve", "mvex", "mvp", "mvp", "mvy", "mxf", "mxv", "mys", "ncor", "nsv", "ntp", "nut", "nuv", "nvc", "ogm", "ogv", "ogx", "orv", "osp", "otrkey", "pac", "par", "pds", "pgi", "photoshow", "piv", "pjs", "playlist", "plproj", "pmf", "pmv", "pns", "ppj", "prel", "pro", "pro4dvd", "pro5dvd", "proqc", "prproj", "prtl", "psb", "psh", "pssd", "pva", "pvr", "pxv", "qt", "qtch", "qtindex", "qtl", "qtm", "qtz", "r3d", "rcd", "rcproject", "rcut", "rdb", "rec", "rm", "rmd", "rmd", "rmp", "rms", "rmv", "rmvb", "roq", "rp", "rsx", "rts", "rts", "rum", "rv", "rvid", "rvl", "sbk", "sbt", "sbz", "scc", "scm", "scm", "scn", "screenflow", "sdv", "sec", "sec", "sedprj", "seq", "sfd", "sfera", "sfvidcap", "siv", "smi", "smi", "smil", "smk", "sml", "smv", "snagproj", "spl", "sqz", "srt", "ssf", "ssm", "stl", "str", "stx", "svi", "swf", "swi", "swt", "tda3mt", "tdt", "tdx", "theater", "thp", "tid", "tivo", "tix", "tod", "tp", "tp0", "tpd", "tpr", "trec", "trp", "ts ", "tsp", "ttxt", "tvlayer", "tvrecording", "tvs", "tvshow", "usf", "usm", "vbc", "vc1", "vcpf", "vcr", "vcv", "vdo", "vdr", "vdx", "veg", "vem", "vep", "vf", "vft", "vfw", "vfz", "vgz", "vid", "video", "viewlet", "viv", "vivo", "vix", "vlab", "vmlf", "vmlt", "vob", "vp3", "vp6", "vp7", "vpj", "vro", "vs4", "vse", "vsp", "vtt", "w32", "wcp", "webm", "wfsp", "wlmp", "wm", "wmd", "wmmp", "wmv", "wmx", "wot", "wp3", "wpl", "wsve", "wtv", "wve", "wvx", "wxp", "xej", "xel", "xesc", "xfl", "xlmv", "xmv", "xvid", "y4m", "yog", "yuv", "zeg", "zm1", "zm2", "zm3", "zmv"];
        var allowedExtension1 = ["avi", "pdf", "doc", "docx", "msg", "xls", "xlsx", "csv", "txt", "zip", "jpg", "jpeg", "png", "gif", "ppt", "pptx", "tif", "tiff", "bmp"];
        if ($.inArray(ext, allowedExtension1) <= 0) {
           return false;
        }
        if (!allowed.length) { return true; }

        for (var i = 0; i < allowed.length; i++) {
            if (allowed[i].toLowerCase() == ext) { return true; }
        }

        return false;
    },
    _formatSize: function (bytes) {
        var i = -1;
        do {
            bytes = bytes / 1024;
            i++;
        } while (bytes > 99);

        return Math.max(bytes, 0.1).toFixed(1) + ['kB', 'MB', 'GB', 'TB', 'PB', 'EB'][i];
    }
};


/**
* Class that creates upload widget with drag-and-drop and file list
* @inherits qq.FileUploaderBasic
*/
qq.FileUploader = function (o) {
    // call parent constructor
    qq.FileUploaderBasic.apply(this, arguments);

    // additional options    
    qq.extend(this._options, {
        element: null,
        // if set, will be used instead of qq-upload-list in template
        listElement: null,
        uploadButtonText: 'Browse',

        template: '<div class="qq-uploader">' +
                '<div class="qq-upload-drop-area"><span>Drop files here to upload</span></div>' +
                '<div class="qq-upload-button btn btn-primary">{uploadButtonText}</div>' +
                '<ul class="qq-upload-list"></ul>' +
             '</div>',

        // template for one item in file list
        fileTemplate: '<li>' +
                '<span class="qq-upload-file"></span>' +
                '<span class="qq-upload-spinner"></span>' +
                '<span class="qq-upload-size"></span>' +
                '<a class="qq-upload-cancel" href="#">Cancel</a>' +
                '<span class="qq-upload-status-text"></span>' +
            '</li>',

        classes: {
            // used to get elements from templates
            button: 'qq-upload-button',
            drop: 'qq-upload-drop-area',
            dropActive: 'qq-upload-drop-area-active',
            list: 'qq-upload-list',

            file: 'qq-upload-file',
            spinner: 'qq-upload-spinner',
            size: 'qq-upload-size',
            cancel: 'qq-upload-cancel',

            // added to list item when upload completes
            // used in css to hide progress spinner
            success: 'qq-upload-success',
            fail: 'qq-upload-fail'
        }
    });
    // overwrite options with user supplied    
    qq.extend(this._options, o);

    // overwrite the upload button text if any
    this._options.template = this._options.template.replace(/\{uploadButtonText\}/g, this._options.uploadButtonText);

    this._element = this._options.element;
    this._element.innerHTML = this._options.template;
    this._listElement = this._options.listElement || this._find(this._element, 'list');

    this._classes = this._options.classes;

    this._button = this._createUploadButton(this._find(this._element, 'button'));

    this._bindCancelEvent();
    this._setupDragDrop();
};

// inherit from Basic Uploader
qq.extend(qq.FileUploader.prototype, qq.FileUploaderBasic.prototype);

qq.extend(qq.FileUploader.prototype, {
    /**
    * Gets one of the elements listed in this._options.classes
    **/
    _find: function (parent, type) {
        var element = qq.getByClass(parent, this._options.classes[type])[0];
        if (!element) {
            throw new Error('element not found ' + type);
        }

        return element;
    },
    _setupDragDrop: function () {
        var self = this,
            dropArea = this._find(this._element, 'drop');

        var dz = new qq.UploadDropZone({
            element: dropArea,
            onEnter: function (e) {
                qq.addClass(dropArea, self._classes.dropActive);
                e.stopPropagation();
            },
            onLeave: function (e) {
                e.stopPropagation();
            },
            onLeaveNotDescendants: function (e) {
                qq.removeClass(dropArea, self._classes.dropActive);
            },
            onDrop: function (e) {
                dropArea.style.display = 'none';
                qq.removeClass(dropArea, self._classes.dropActive);
                self._uploadFileList(e.dataTransfer.files);
            }
        });

        dropArea.style.display = 'none';

        qq.attach(document, 'dragenter', function (e) {
            if (!dz._isValidFileDrag(e)) return;

            dropArea.style.display = 'block';
        });
        qq.attach(document, 'dragleave', function (e) {
            if (!dz._isValidFileDrag(e)) return;

            var relatedTarget = document.elementFromPoint(e.clientX, e.clientY);
            // only fire when leaving document out
            if (!relatedTarget || relatedTarget.nodeName == "HTML") {
                dropArea.style.display = 'none';
            }
        });
    },
    _onSubmit: function (id, fileName) {
        qq.FileUploaderBasic.prototype._onSubmit.apply(this, arguments);
        this._addToList(id, fileName);
    },
    _onProgress: function (id, fileName, loaded, total) {
        qq.FileUploaderBasic.prototype._onProgress.apply(this, arguments);

        var item = this._getItemByFileId(id);
        var size = this._find(item, 'size');
        size.style.display = 'inline';

        var text;
        if (loaded != total) {
            text = Math.round(loaded / total * 100) + '% from ' + this._formatSize(total);
        } else {
            text = this._formatSize(total);
        }

        qq.setText(size, text);
    },
    _onComplete: function (id, fileName, result) {
        qq.FileUploaderBasic.prototype._onComplete.apply(this, arguments);

        // mark completed
        var item = this._getItemByFileId(id);
        qq.remove(this._find(item, 'cancel'));
        qq.remove(this._find(item, 'spinner'));
        if (typeof (result.IndividualUpload) !== "undefined" && result.IndividualUpload) {
            if (result.Status) {
                qq.addClass(item, this._classes.success);
                setTimeout(function () {
                    $(item).fadeOut(function () {
                        $(item).remove();
                    })
                }, 1000);
            } else {
                qq.addClass(item, this._classes.fail);
                $(item).find(".qq-upload-status-text").text("Fail");
                setTimeout(function () {
                    $(item).fadeOut(function () {
                        $(item).remove();
                    })
                }, 1000);
            }
        }
        else if (typeof (result.FileId) !== "undefined" && result.FileId != null && result.FileId != '') {
            qq.addClass(item, this._classes.success);
            var uploaderElementId = $(item).parents(".qq-uploader").parent().attr("id");
            $(item).find(".qq-upload-file").text(result.FileName);
            $(item).find(".qq-upload-status-text").html('<i data-url="' + result.FileURL + '" onclick="DownloadUploadedFile(this);" class="fa fa-download"></i>' +
                "<i data-id='" + result.FileId + "' onclick='" + uploaderElementId + "RemoveImage(this);' class='fa fa-trash-o'></i>");
        } else {
            qq.addClass(item, this._classes.fail);
            $(item).find(".qq-upload-status-text").text("Fail");
            setTimeout(function () {
                $(item).fadeOut(function () {
                    $(item).remove();
                })
            }, 1000);
        }
    },
    _addToList: function (id, fileName) {
        var item = qq.toElement(this._options.fileTemplate);
        item.qqFileId = id;

        var fileElement = this._find(item, 'file');
        qq.setText(fileElement, this._formatFileName(fileName));
        this._find(item, 'size').style.display = 'none';

        this._listElement.appendChild(item);
    },
    _getItemByFileId: function (id) {
        var item = this._listElement.firstChild;

        // there can't be txt nodes in dynamically created list
        // and we can  use nextSibling
        while (item) {
            if (item.qqFileId == id) return item;
            item = item.nextSibling;
        }
    },
    /**
    * delegate click event for cancel link 
    **/
    _bindCancelEvent: function () {
        var self = this,
            list = this._listElement;

        qq.attach(list, 'click', function (e) {
            e = e || window.event;
            var target = e.target || e.srcElement;

            if (qq.hasClass(target, self._classes.cancel)) {
                qq.preventDefault(e);

                var item = target.parentNode;
                self._handler.cancel(item.qqFileId);
                qq.remove(item);
            }
        });
    }
});

qq.UploadDropZone = function (o) {
    this._options = {
        element: null,
        onEnter: function (e) { },
        onLeave: function (e) { },
        // is not fired when leaving element by hovering descendants   
        onLeaveNotDescendants: function (e) { },
        onDrop: function (e) { }
    };
    qq.extend(this._options, o);

    this._element = this._options.element;

    this._disableDropOutside();
    this._attachEvents();
};

qq.UploadDropZone.prototype = {
    _disableDropOutside: function (e) {
        // run only once for all instances
        if (!qq.UploadDropZone.dropOutsideDisabled) {

            qq.attach(document, 'dragover', function (e) {
                if (e.dataTransfer) {
                    e.dataTransfer.dropEffect = 'none';
                    e.preventDefault();
                }
            });

            qq.UploadDropZone.dropOutsideDisabled = true;
        }
    },
    _attachEvents: function () {
        var self = this;

        qq.attach(self._element, 'dragover', function (e) {
            if (!self._isValidFileDrag(e)) return;

            var effect = e.dataTransfer.effectAllowed;
            if (effect == 'move' || effect == 'linkMove') {
                e.dataTransfer.dropEffect = 'move'; // for FF (only move allowed)    
            } else {
                e.dataTransfer.dropEffect = 'copy'; // for Chrome
            }

            e.stopPropagation();
            e.preventDefault();
        });

        qq.attach(self._element, 'dragenter', function (e) {
            if (!self._isValidFileDrag(e)) return;

            self._options.onEnter(e);
        });

        qq.attach(self._element, 'dragleave', function (e) {
            if (!self._isValidFileDrag(e)) return;

            self._options.onLeave(e);

            var relatedTarget = document.elementFromPoint(e.clientX, e.clientY);
            // do not fire when moving a mouse over a descendant
            if (qq.contains(this, relatedTarget)) return;

            self._options.onLeaveNotDescendants(e);
        });

        qq.attach(self._element, 'drop', function (e) {
            if (!self._isValidFileDrag(e)) return;

            e.preventDefault();
            self._options.onDrop(e);
        });
    },
    _isValidFileDrag: function (e) {
        var dt = e.dataTransfer,
        // do not check dt.types.contains in webkit, because it crashes safari 4            
            isWebkit = navigator.userAgent.indexOf("AppleWebKit") > -1;

        // dt.effectAllowed is none in Safari 5
        // dt.types.contains check is for firefox            
        return dt && dt.effectAllowed != 'none' &&
            (dt.files || (!isWebkit && dt.types.contains && dt.types.contains('Files')));

    }
};

qq.UploadButton = function (o) {
    this._options = {
        element: null,
        // if set to true adds multiple attribute to file input      
        multiple: false,
        // name attribute of file input
        name: 'file',
        onChange: function (input) { },
        hoverClass: 'qq-upload-button-hover',
        focusClass: 'qq-upload-button-focus'
    };

    qq.extend(this._options, o);

    this._element = this._options.element;

    // make button suitable container for input
    qq.css(this._element, {
        position: 'relative',
        overflow: 'hidden',
        // Make sure browse button is in the right side
        // in Internet Explorer
        direction: 'ltr'
    });

    this._input = this._createInput();
};

qq.UploadButton.prototype = {
    /* returns file input element */
    getInput: function () {
        return this._input;
    },
    /* cleans/recreates the file input */
    reset: function () {
        if (this._input.parentNode) {
            qq.remove(this._input);
        }

        qq.removeClass(this._element, this._options.focusClass);
        this._input = this._createInput();
    },
    _createInput: function () {
        var input = document.createElement("input");

        if (this._options.multiple) {
            input.setAttribute("multiple", "multiple");
        }

        input.setAttribute("type", "file");
        input.setAttribute("name", this._options.name);

        qq.css(input, {
            position: 'absolute',
            right: 0,
            top: 0,
            fontFamily: 'Arial',

            //fontSize: '118px',
            margin: 0,
            padding: 0,
            cursor: 'pointer',
            width: '213px',
            opacity: '0'




        });

        this._element.appendChild(input);

        var self = this;
        qq.attach(input, 'change', function () {
            self._options.onChange(input);
        });

        qq.attach(input, 'mouseover', function () {
            qq.addClass(self._element, self._options.hoverClass);
        });
        qq.attach(input, 'mouseout', function () {
            qq.removeClass(self._element, self._options.hoverClass);
        });
        qq.attach(input, 'focus', function () {
            qq.addClass(self._element, self._options.focusClass);
        });
        qq.attach(input, 'blur', function () {
            qq.removeClass(self._element, self._options.focusClass);
        });

        // IE and Opera, unfortunately have 2 tab stops on file input
        // which is unacceptable in our case, disable keyboard access
        if (window.attachEvent) {
            // it is IE or Opera
            input.setAttribute('tabIndex', "-1");
        }

        return input;
    }
};

/**
* Class for uploading files, uploading itself is handled by child classes
*/
qq.UploadHandlerAbstract = function (o) {
    this._options = {
        debug: false,
        action: '/upload.php',
        // maximum number of concurrent uploads        
        maxConnections: 999,
        onProgress: function (id, fileName, loaded, total) { },
        onComplete: function (id, fileName, response) { },
        onCancel: function (id, fileName) { }
    };
    qq.extend(this._options, o);

    this._queue = [];
    // params for files in queue
    this._params = [];
};
qq.UploadHandlerAbstract.prototype = {
    log: function (str) {
        //if (this._options.debug && window.console) console.log('[uploader] ' + str);
    },
    /**
    * Adds file or file input to the queue
    * @returns id
    **/
    add: function (file) { },
    /**
    * Sends the file identified by id and additional query params to the server
    */
    upload: function (id, params) {
        var len = this._queue.push(id);

        var copy = {};
        qq.extend(copy, params);
        this._params[id] = copy;

        // if too many active uploads, wait...
        if (len <= this._options.maxConnections) {
            this._upload(id, this._params[id]);
        }
    },
    /**
    * Cancels file upload by id
    */
    cancel: function (id) {
        this._cancel(id);
        this._dequeue(id);
    },
    /**
    * Cancells all uploads
    */
    cancelAll: function () {
        for (var i = 0; i < this._queue.length; i++) {
            this._cancel(this._queue[i]);
        }
        this._queue = [];
    },
    /**
    * Returns name of the file identified by id
    */
    getName: function (id) { },
    /**
    * Returns size of the file identified by id
    */
    getSize: function (id) { },
    /**
    * Returns id of files being uploaded or
    * waiting for their turn
    */
    getQueue: function () {
        return this._queue;
    },
    /**
    * Actual upload method
    */
    _upload: function (id) { },
    /**
    * Actual cancel method
    */
    _cancel: function (id) { },
    /**
    * Removes element from queue, starts upload of next
    */
    _dequeue: function (id) {
        var i = qq.indexOf(this._queue, id);
        this._queue.splice(i, 1);

        var max = this._options.maxConnections;

        if (this._queue.length >= max && i < max) {
            var nextId = this._queue[max - 1];
            this._upload(nextId, this._params[nextId]);
        }
    }
};

/**
* Class for uploading files using form and iframe
* @inherits qq.UploadHandlerAbstract
*/
qq.UploadHandlerForm = function (o) {
    qq.UploadHandlerAbstract.apply(this, arguments);

    this._inputs = {};
};
// @inherits qq.UploadHandlerAbstract
qq.extend(qq.UploadHandlerForm.prototype, qq.UploadHandlerAbstract.prototype);

qq.extend(qq.UploadHandlerForm.prototype, {
    add: function (fileInput) {
        fileInput.setAttribute('name', 'qqfile');
        var id = 'qq-upload-handler-iframe' + qq.getUniqueId();

        this._inputs[id] = fileInput;

        // remove file input from DOM
        if (fileInput.parentNode) {
            qq.remove(fileInput);
        }

        return id;
    },
    getName: function (id) {
        // get input value and remove path to normalize
        return this._inputs[id].value.replace(/.*(\/|\\)/, "");
    },
    _cancel: function (id) {
        this._options.onCancel(id, this.getName(id));

        delete this._inputs[id];

        var iframe = document.getElementById(id);
        if (iframe) {
            // to cancel request set src to something else
            // we use src="javascript:false;" because it doesn't
            // trigger ie6 prompt on https
            iframe.setAttribute('src', 'javascript:false;');

            qq.remove(iframe);
        }
    },
    _upload: function (id, params) {
        var input = this._inputs[id];

        if (!input) {
            throw new Error('file with passed id was not added, or already uploaded or cancelled');
        }

        var fileName = this.getName(id);

        var iframe = this._createIframe(id);
        var form = this._createForm(iframe, params);
        form.appendChild(input);

        var self = this;
        this._attachLoadEvent(iframe, function () {
            self.log('iframe loaded');

            var response = self._getIframeContentJSON(iframe);

            self._options.onComplete(id, fileName, response);
            self._dequeue(id);

            delete self._inputs[id];
            // timeout added to fix busy state in FF3.6
            setTimeout(function () {
                qq.remove(iframe);
            }, 1);
        });

        form.submit();
        qq.remove(form);

        return id;
    },
    _attachLoadEvent: function (iframe, callback) {
        qq.attach(iframe, 'load', function () {
            // when we remove iframe from dom
            // the request stops, but in IE load
            // event fires
            if (!iframe.parentNode) {
                return;
            }

            // fixing Opera 10.53
            if (iframe.contentDocument &&
                iframe.contentDocument.body &&
                iframe.contentDocument.body.innerHTML == "false") {
                // In Opera event is fired second time
                // when body.innerHTML changed from false
                // to server response approx. after 1 sec
                // when we upload file with iframe
                return;
            }

            callback();
        });
    },
    /**
    * Returns json object received by iframe from server.
    */
    _getIframeContentJSON: function (iframe) {
        // iframe.contentWindow.document - for IE<7
        var doc = iframe.contentDocument ? iframe.contentDocument : iframe.contentWindow.document,
            response;

        this.log("converting iframe's innerHTML to JSON");
        this.log("innerHTML = " + doc.body.innerHTML);

        try {
            response = eval("(" + doc.body.innerHTML + ")");
        } catch (err) {
            response = {};
        }

        return response;
    },
    /**
    * Creates iframe with unique name
    */
    _createIframe: function (id) {
        // We can't use following code as the name attribute
        // won't be properly registered in IE6, and new window
        // on form submit will open
        // var iframe = document.createElement('iframe');
        // iframe.setAttribute('name', id);

        var iframe = qq.toElement('<iframe src="javascript:false;" name="' + id + '" />');
        // src="javascript:false;" removes ie6 prompt on https

        iframe.setAttribute('id', id);

        iframe.style.display = 'none';
        document.body.appendChild(iframe);

        return iframe;
    },
    /**
    * Creates form, that will be submitted to iframe
    */
    _createForm: function (iframe, params) {
        // We can't use the following code in IE6
        // var form = document.createElement('form');
        // form.setAttribute('method', 'post');
        // form.setAttribute('enctype', 'multipart/form-data');
        // Because in this case file won't be attached to request
        var form = qq.toElement('<form method="post" enctype="multipart/form-data"></form>');

        var queryString = qq.obj2url(params, this._options.action);

        form.setAttribute('action', queryString);
        form.setAttribute('target', iframe.name);
        form.style.display = 'none';
        document.body.appendChild(form);

        return form;
    }
});

/**
* Class for uploading files using xhr
* @inherits qq.UploadHandlerAbstract
*/
qq.UploadHandlerXhr = function (o) {
    qq.UploadHandlerAbstract.apply(this, arguments);

    this._files = [];
    this._xhrs = [];

    // current loaded size in bytes for each file 
    this._loaded = [];
};

// static method
qq.UploadHandlerXhr.isSupported = function () {
    var input = document.createElement('input');
    input.type = 'file';

    return (
        'multiple' in input &&
        typeof File != "undefined" &&
        typeof (new XMLHttpRequest()).upload != "undefined");
};

// @inherits qq.UploadHandlerAbstract
qq.extend(qq.UploadHandlerXhr.prototype, qq.UploadHandlerAbstract.prototype)

qq.extend(qq.UploadHandlerXhr.prototype, {
    /**
    * Adds file to the queue
    * Returns id to use with upload, cancel
    **/
    add: function (file) {
        if (!(file instanceof File)) {
            throw new Error('Passed obj in not a File (in qq.UploadHandlerXhr)');
        }

        return this._files.push(file) - 1;
    },
    getName: function (id) {
        var file = this._files[id];
        // fix missing name in Safari 4
        return file.fileName != null ? file.fileName : file.name;
    },
    getSize: function (id) {
        var file = this._files[id];
        return file.fileSize != null ? file.fileSize : file.size;
    },
    /**
    * Returns uploaded bytes for file identified by id 
    */
    getLoaded: function (id) {
        return this._loaded[id] || 0;
    },
    /**
    * Sends the file identified by id and additional query params to the server
    * @param {Object} params name-value string pairs
    */
    _upload: function (id, params) {
        var file = this._files[id],
            name = this.getName(id),
            size = this.getSize(id);

        this._loaded[id] = 0;

        var xhr = this._xhrs[id] = new XMLHttpRequest();
        var self = this;

        xhr.upload.onprogress = function (e) {
            if (e.lengthComputable) {
                self._loaded[id] = e.loaded;
                self._options.onProgress(id, name, e.loaded, e.total);
            }
        };

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                self._onComplete(id, xhr);
            }
        };

        // build query string
        params = params || {};
        params['qqfile'] = name;
        var queryString = qq.obj2url(params, this._options.action);

        xhr.open("POST", queryString, true);
        xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        xhr.setRequestHeader("X-File-Name", encodeURIComponent(name));
        xhr.setRequestHeader("Content-Type", "application/octet-stream");
        xhr.send(file);
    },
    _onComplete: function (id, xhr) {
        // the request was aborted/cancelled
        if (!this._files[id]) return;

        var name = this.getName(id);
        var size = this.getSize(id);

        this._options.onProgress(id, name, size, size);

        if (xhr.status == 200) {
            this.log("xhr - server response received");
            this.log("responseText = " + xhr.responseText);

            var response;

            try {
                response = eval("(" + xhr.responseText + ")");
            } catch (err) {
                response = {};
            }

            this._options.onComplete(id, name, response);

        } else {
            this._options.onComplete(id, name, {});
        }

        this._files[id] = null;
        this._xhrs[id] = null;
        this._dequeue(id);
    },
    _cancel: function (id) {
        this._options.onCancel(id, this.getName(id));

        this._files[id] = null;

        if (this._xhrs[id]) {
            this._xhrs[id].abort();
            this._xhrs[id] = null;
        }
    }
});

Encoder = {

    // When encoding do we convert characters into html or numerical entities
    EncodeType: "entity",  // entity OR numerical

    isEmpty: function (val) {
        if (val) {
            return ((val === null) || val.length == 0 || /^\s+$/.test(val));
        } else {
            return true;
        }
    },

    // arrays for conversion from HTML Entities to Numerical values
    arr1: ['&nbsp;', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&Agrave;', '&Aacute;', '&Acirc;', '&Atilde;', '&Auml;', '&Aring;', '&AElig;', '&Ccedil;', '&Egrave;', '&Eacute;', '&Ecirc;', '&Euml;', '&Igrave;', '&Iacute;', '&Icirc;', '&Iuml;', '&ETH;', '&Ntilde;', '&Ograve;', '&Oacute;', '&Ocirc;', '&Otilde;', '&Ouml;', '&times;', '&Oslash;', '&Ugrave;', '&Uacute;', '&Ucirc;', '&Uuml;', '&Yacute;', '&THORN;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&OElig;', '&oelig;', '&Scaron;', '&scaron;', '&Yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&Dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&Alpha;', '&Beta;', '&Gamma;', '&Delta;', '&Epsilon;', '&Zeta;', '&Eta;', '&Theta;', '&Iota;', '&Kappa;', '&Lambda;', '&Mu;', '&Nu;', '&Xi;', '&Omicron;', '&Pi;', '&Rho;', '&Sigma;', '&Tau;', '&Upsilon;', '&Phi;', '&Chi;', '&Psi;', '&Omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&Prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&lArr;', '&uArr;', '&rArr;', '&dArr;', '&hArr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;'],
    arr2: ['&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;'],

    // Convert HTML entities into numerical entities
    HTML2Numerical: function (s) {
        return this.swapArrayVals(s, this.arr1, this.arr2);
    },

    // Convert Numerical entities into HTML entities
    NumericalToHTML: function (s) {
        return this.swapArrayVals(s, this.arr2, this.arr1);
    },


    // Numerically encodes all unicode characters
    numEncode: function (s) {
        if (this.isEmpty(s)) return "";

        var a = [],
			l = s.length;

        for (var i = 0; i < l; i++) {
            var c = s.charAt(i);
            if (c < " " || c > "~") {
                a.push("&#");
                a.push(c.charCodeAt()); //numeric value of code point 
                a.push(";");
            } else {
                a.push(c);
            }
        }

        return a.join("");
    },

    // HTML Decode numerical and HTML entities back to original values
    htmlDecode: function (s) {

        var c, m, d = s;

        if (this.isEmpty(d)) return "";

        // convert HTML entites back to numerical entites first
        d = this.HTML2Numerical(d);

        // look for numerical entities &#34;
        arr = d.match(/&#[0-9]{1,5};/g);

        // if no matches found in string then skip
        if (arr != null) {
            for (var x = 0; x < arr.length; x++) {
                m = arr[x];
                c = m.substring(2, m.length - 1); //get numeric part which is refernce to unicode character
                // if its a valid number we can decode
                if (c >= -32768 && c <= 65535) {
                    // decode every single match within string
                    d = d.replace(m, String.fromCharCode(c));
                } else {
                    d = d.replace(m, ""); //invalid so replace with nada
                }
            }
        }

        return d;
    },

    // encode an input string into either numerical or HTML entities
    htmlEncode: function (s, dbl) {

        if (this.isEmpty(s)) return "";

        // do we allow double encoding? E.g will &amp; be turned into &amp;amp;
        dbl = dbl || false; //default to prevent double encoding

        // if allowing double encoding we do ampersands first
        if (dbl) {
            if (this.EncodeType == "numerical") {
                s = s.replace(/&/g, "&#38;");
            } else {
                s = s.replace(/&/g, "&amp;");
            }
        }

        // convert the xss chars to numerical entities ' " < >
        s = this.XSSEncode(s, false);

        if (this.EncodeType == "numerical" || !dbl) {
            // Now call function that will convert any HTML entities to numerical codes
            s = this.HTML2Numerical(s);
        }

        // Now encode all chars above 127 e.g unicode
        s = this.numEncode(s);

        // now we know anything that needs to be encoded has been converted to numerical entities we
        // can encode any ampersands & that are not part of encoded entities
        // to handle the fact that I need to do a negative check and handle multiple ampersands &&&
        // I am going to use a placeholder

        // if we don't want double encoded entities we ignore the & in existing entities
        if (!dbl) {
            s = s.replace(/&#/g, "##AMPHASH##");

            if (this.EncodeType == "numerical") {
                s = s.replace(/&/g, "&#38;");
            } else {
                s = s.replace(/&/g, "&amp;");
            }

            s = s.replace(/##AMPHASH##/g, "&#");
        }

        // replace any malformed entities
        s = s.replace(/&#\d*([^\d;]|$)/g, "$1");

        if (!dbl) {
            // safety check to correct any double encoded &amp;
            s = this.correctEncoding(s);
        }

        // now do we need to convert our numerical encoded string into entities
        if (this.EncodeType == "entity") {
            s = this.NumericalToHTML(s);
        }

        return s;
    },

    // Encodes the basic 4 characters used to malform HTML in XSS hacks
    XSSEncode: function (s, en) {
        if (!this.isEmpty(s)) {
            en = en || true;
            // do we convert to numerical or html entity?
            if (en) {
                s = s.replace(/\'/g, "&#39;"); //no HTML equivalent as &apos is not cross browser supported
                s = s.replace(/\"/g, "&quot;");
                s = s.replace(/</g, "&lt;");
                s = s.replace(/>/g, "&gt;");
            } else {
                s = s.replace(/\'/g, "&#39;"); //no HTML equivalent as &apos is not cross browser supported
                s = s.replace(/\"/g, "&#34;");
                s = s.replace(/</g, "&#60;");
                s = s.replace(/>/g, "&#62;");
            }
            return s;
        } else {
            return "";
        }
    },

    // returns true if a string contains html or numerical encoded entities
    hasEncoded: function (s) {
        if (/&#[0-9]{1,5};/g.test(s)) {
            return true;
        } else if (/&[A-Z]{2,6};/gi.test(s)) {
            return true;
        } else {
            return false;
        }
    },

    // will remove any unicode characters
    stripUnicode: function (s) {
        return s.replace(/[^\x20-\x7E]/g, "");

    },

    // corrects any double encoded &amp; entities e.g &amp;amp;
    correctEncoding: function (s) {
        return s.replace(/(&amp;)(amp;)+/, "$1");
    },


    // Function to loop through an array swaping each item with the value from another array e.g swap HTML entities with Numericals
    swapArrayVals: function (s, arr1, arr2) {
        if (this.isEmpty(s)) return "";
        var re;
        if (arr1 && arr2) {
            //ShowDebug("in swapArrayVals arr1.length = " + arr1.length + " arr2.length = " + arr2.length)
            // array lengths must match
            if (arr1.length == arr2.length) {
                for (var x = 0, i = arr1.length; x < i; x++) {
                    re = new RegExp(arr1[x], 'g');
                    s = s.replace(re, arr2[x]); //swap arr1 item with matching item from arr2	
                }
            }
        }
        return s;
    },

    inArray: function (item, arr) {
        for (var i = 0, x = arr.length; i < x; i++) {
            if (arr[i] === item) {
                return i;
            }
        }
        return -1;
    }

}
function DownloadUploadedFile(ele, callback) {
    var url = $(ele).attr("data-url");
    var appName = $(ele).parents(".qq-uploader").parent().attr("data-appname");
    if (typeof (appName) === 'undefined') {
        appName = $(ele).attr("data-appname");
    }
    if (typeof (url) !== "undefined" && url != null && url != '') {
        $.ajax({
            url: BASEPATHURL + '/Master/VerifyDownloadFile',
            method: "POST",
            data: { url: url, applicationName: appName },
            beforeSend: function () {
                $("#loading").show();
            },
            success: function (data) {
                if (data.Status) {
                    $("#download_form").remove();
                    var download_form = document.createElement('FORM');
                    download_form.setAttribute('style', 'display:none;visibility:hidden;');
                    download_form.className = "hide";
                    download_form.name = 'download_form';
                    download_form.id = "download_form";
                    download_form.method = 'POST';
                    download_form.action = BASEPATHURL + '/Master/DownloadFile';
                    var urlElement = document.createElement('INPUT');
                    urlElement.type = 'hidden';
                    urlElement.name = 'url';
                    urlElement.id = 'url';
                    urlElement.value = url;
                    download_form.appendChild(urlElement);
                    var appElement = document.createElement('INPUT');
                    appElement.type = 'hidden';
                    appElement.name = 'applicationName';
                    appElement.id = 'applicationName';
                    appElement.value = appName;
                    download_form.appendChild(appElement);
                    var currenturlElement = document.createElement('INPUT');
                    currenturlElement.type = 'hidden';
                    currenturlElement.name = 'currentLocation';
                    currenturlElement.id = 'currentLocation';
                    currenturlElement.value = window.location;
                    download_form.appendChild(currenturlElement);
                    var btnSubmit = document.createElement('INPUT');
                    btnSubmit.id = "btnDownloadSubmitFileUploader";
                    btnSubmit.type = 'submit';
                    download_form.appendChild(btnSubmit);
                    //download_form.submit();
                    $(download_form).appendTo("body");
                    $("#btnDownloadSubmitFileUploader").click();
                    $("#download_form").remove();
                    if (typeof (callback) !== "undefined") {
                        callback();
                    }
                }
                else {
                    AlertModal("Error", "File not found for downloading.");
                }
                $("#loading").hide();
            }
        });
    }
}
function BindFileList(eleSelectorId, uploaderSelectorId) {
    var element = $("#" + eleSelectorId);
    var uploader = $("#" + uploaderSelectorId);
    var uploadList = [];
    $(element).addClass("validate-file");
    if (element.length > 0 && uploader.length > 0 && $.trim(element.val()) != '' && $.trim(element.val()) != '[]') {
        uploadList = JSON.parse(element.val());
        $(uploadList).each(function (i, item) {
            var uploadItem = $('<li class="qq-upload-success"><span class="qq-upload-file">' + item.FileName + '</span>' +
                '<span class="qq-upload-size"></span><span class="qq-upload-status-text">' +
                '<i data-url="' + item.FileURL + '" onclick="DownloadUploadedFile(this);" class="fa fa-download"></i>' +
                '<i data-id="' + item.FileId + '" onclick="' + uploaderSelectorId + 'RemoveImage(this);" class="fa fa-trash-o"></i>' +
                '</span></li>');
            uploadItem.appendTo("#" + uploaderSelectorId + " ul.qq-upload-list");
        });
    }
    return typeof (uploadList) === 'undefined' || uploadList == null ? [] : uploadList;
}