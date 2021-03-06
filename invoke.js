<script>
function invoke() {
    this.externalmethodnames = JSON.parse(window.external.getmethodslist()).value;
    if (arguments.length == 0 || this.externalmethodnames.indexOf(arguments[0]) == -1) {
        return false;
    }
    try {
        var args = Array.prototype.slice.call(arguments, 1);
        var result = window.external.callmethod(arguments[0], JS2VBArray(args));
        var ret = IsValidJSONString(result)
        return ret ? ret : result;
    } catch (ex) { return "invoke error: " + ex; }
}
csharp = new invoke();
for (var i in csharp.externalmethodnames) {
  try {
    csharp[csharp.externalmethodnames[i]] = eval("(function(){\
  try {\
      var args = Array.prototype.slice.call(arguments);\
      var result = window.external.callmethod('"+ csharp.externalmethodnames[i] + "', JS2VBArray(args));\
      var ret = IsValidJSONString(result);\
      return ret ? ret : result;\
  } catch (ex) { alert('invoke error: ' + ex); }\
})");
  } catch (ex) { alert(ex);break; }
}
function JS2VBArray(objJSArray) {
    var dictionary = new ActiveXObject("Scripting.Dictionary");
    for (var i = 0; i < objJSArray.length; i++) {
        dictionary.add(i, objJSArray[i]);
    }

    return dictionary.Items();
}
function IsValidJSONString(str) {
    try {
        var a = JSON.parse(str);
    } catch (e) {
        return false;
    }
    finally { return a };
}
</script>
