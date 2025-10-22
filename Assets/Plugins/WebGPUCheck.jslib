mergeInto(LibraryManager.library, {
  IsWebGPUAvailable: function () {
    return navigator.gpu ? 1 : 0;
  },
});
