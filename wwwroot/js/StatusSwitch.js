
window.getStatusClass = function (status) {   /// Global function to map status to CSS class 
    if (!status) return "table-pending";

    status = status.trim().toLowerCase();//dicards whitespaces+makes strinsg case-insensitive

    switch (status) {   //status returning respective classes
        case "accepted": return "table-accept";
        case "reject": return "table-reject";
        case "transit": return "table-pending";
        case "onsite": return "table-pending";
        case "accept transit": return "table-accept-transit";
        case "reject transit": return "table-reject-transit";
        case "accept onsite": return "table-accept-onsite";
        case "reject onsite": return "table-reject-onsite";
        case "complete": return "table-complete";
        case "repairing": return "table-repairing";
        case "send back": return "table-sendback";
        default: return "table-pending";
    }
};
``
// Global event handler for Accept/Reject actions
$(document).on("click", ".action-btn", function () {
    var id = $(this).data("id");        //get data id+data action by click event
    var action = $(this).data("action");
    var button = $(this);

    if (!confirm("Are you sure you want to " + action + " this request?")) return;  //confirmation msg according to action

    $.ajax({
        url: "/FormReqDb/UpdateStatus",
        type: "POST",
        data: { id: id, actionType: action },//passes id+action to server
        success: function (response) {
            if (response.success) {
                var newStatus = response.newStatus.trim().toLowerCase(); //new sts value->case-insensitive
                var statusCell = $(".status-cell[data-id='" + id + "']");
                var newClass = getStatusClass(newStatus); 

                statusCell.removeClass().addClass("status-cell " + newClass).text(response.newStatus);//removes existing sts->updates status+color

                var row = $("#row-" + id);
                row.find(".accept-btn").prop("disabled", newStatus.includes("accept"));
                row.find(".reject-btn").prop("disabled", newStatus.includes("reject"));
            } else {
                alert(response.message || "Something went wrong.");
            }
        },
        error: function () {
            alert("Failed to update status.");
        }
    });
});
