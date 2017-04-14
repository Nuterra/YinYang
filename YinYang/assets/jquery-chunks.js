// http://stackoverflow.com/questions/3366529/wrap-every-3-divs-in-a-div
$.fn.chunk = function (size) {
    var arr = [];
    for (var i = 0; i < this.length; i += size) {
        arr.push(this.slice(i, i + size));
    }
    return this.pushStack(arr, "chunk", size);
}