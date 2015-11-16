
import ko = require("knockout");

module HashObservable {
    export var params = ko.observable();
    export var hash = ko.observable<string>();
   export var route = ko.observable();
}
var hashChange = () => {


    var hash = window.location.hash;


    var idx = hash.indexOf('?');
    var queryString = "";
    if (idx > -1) {
        queryString = hash.substr(idx + 1);
        hash = hash.substr(0, idx);
    } else {
        queryString = hash.substr(1);
    }

    var params = null,
        regex = /([^&=]+)=([^&]*)/g,
        m;

    while (m = regex.exec(queryString)) {
        params = params || {};
        params[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
    }


    HashObservable.hash(hash.toLowerCase());
    HashObservable.route(HashObservable.hash().substr(1).match(/[^/]+/g));
    HashObservable.params(params);
};
window.addEventListener("hashchange", hashChange, false);
hashChange();

export = HashObservable;