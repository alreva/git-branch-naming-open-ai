window.clipboardCopy = {
  copyText: function(text) {
    navigator.clipboard.writeText(text)
      .then(function () {
        console.log("Copied text: " + text);
      })
      .catch(function (error) {
        alert(error);
      });
  }
};