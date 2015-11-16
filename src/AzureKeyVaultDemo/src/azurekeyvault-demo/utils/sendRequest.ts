
function sendRequest(method, url, token, done, err, data = null) {
    var xhr = new XMLHttpRequest();
    xhr.open(method, url, true);
    xhr.setRequestHeader("Authorization", "bearer " + token);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.onload = function (e) {
        if (xhr.readyState === 4) {
            if (xhr.status >= 200 && xhr.status < 300) {
                done(JSON.parse(xhr.responseText));
            } else {
                console.error(xhr.statusText);
            }
        }
    };
    xhr.onerror = function (e) {
        console.error(xhr.statusText);
    };
    xhr.send(data);
}
export = sendRequest;