mergeInto(LibraryManager.library, {
  SendImageToJS: function (str) {
    console.log("Hello, world!");
    
    var base64 = UTF8ToString(str);
    var bytes = atob(base64);
    var len = bytes.length;
    var buffer = new ArrayBuffer(len);
    var view = new Uint8Array(buffer);
    for (var i = 0; i < len; i++) {
        view[i] = bytes.charCodeAt(i);
    }

    var imageBlob = new Blob([buffer], { type: 'image/png' });

    var formData = new FormData();
    formData.append('image', imageBlob, 'screenshot.png');

    fetch('https://charremarc.fr/Images/upload.php', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => console.log(data))
    .catch(error => console.error(error));
  },
});
