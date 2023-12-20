mergeInto(LibraryManager.library, {
  SendImageToJS: function (base64Image, filename) {

    var base64 = UTF8ToString(base64Image);
    var bytes = atob(base64);
    var len = bytes.length;
    var buffer = new ArrayBuffer(len);
    var view = new Uint8Array(buffer);
    for (var i = 0; i < len; i++) {
        view[i] = bytes.charCodeAt(i);
    }

    var imageBlob = new Blob([buffer], { type: 'image/png' });

    var formData = new FormData();
    var name = UTF8ToString(filename);
    formData.append('image', imageBlob, name + '.png');

    fetch('https://charremarc.fr/Images/upload.php', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => console.log(data))
    .catch(error => console.error(error));
  },
});
