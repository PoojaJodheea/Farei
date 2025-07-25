window.getStatusClass = function (status) {
    if (!status) return "table-pending";

    status = status.trim().toLowerCase();

    switch (status) {
        case "pending":
        case "pending request":
        case "final request":
        case "transit request":
        case "onsite request":
            return "table-pending";

        case "accepted":
        case "accept":
            return "table-accept";

        case "rejected":
        case "reject":
            return "table-reject";

        case "transitting":
        case "accept transit":
            return "table-accept-transit";

        case "reject transit":
            return "table-reject-transit";

        case "accept onsite":
            return "table-accept-onsite";

        case "reject onsite":
            return "table-reject-onsite";

        case "repairing":
        case "start repairing":
            return "table-repairing";

        case "send back":
        
            return "table-sendback";
        case "return":
        case "complete":
            return "table-complete";

        default:
            return "table-pending";
    }
};

// Global event handler for Accept/Reject actions
$(document).on("click", ".action-btn", function () {
    var id = $(this).data("id");
    var action = $(this).data("action");
    var button = $(this); // clicked button

    if (action === "reject" && !confirm("Are you sure you want to reject this request?")) {
        return;
    }

    
    $.ajax({
        url: "/FormReqDb/UpdateStatus",
        type: "POST",
        data: { id: id, actionType: action },
        success: function (response) {
            if (response.success) {
                var newStatus = response.newStatus.trim().toLowerCase();

                // Update the status cell text and class
                $(".status-cell[data-id='" + id + "']")
                    .text(response.newStatus)
                    .removeClass()
                    .addClass("status-cell " + getStatusClass(newStatus));
                console.log("Disabling button for ID:", id, button);
                // Disable the button that was clicked (Accept or Reject)
                button.prop("disabled", true);
            } else {
                alert(response.message || "Something went wrong.");
            }
        }


    });
});
