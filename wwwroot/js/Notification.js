const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationSetting")
    .build();

connection.on("ReceiveNotification", (title, message) => {
    const notifArea = document.getElementById("notif-area");
    const notifCount = document.getElementById("notif-count");

    const item = document.createElement("li");
    item.className = "dropdown-item";
    item.innerHTML = `<strong>${title}</strong><br>${message}`;
    notifArea.prepend(item);

    notifCount.style.display = "inline-block";
    notifCount.innerText = notifArea.childElementCount;
});

connection.start().catch(err => console.error("SignalR connection failed: ", err));
