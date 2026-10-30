document.addEventListener("DOMContentLoaded", function () {
    var expenseName = document.querySelector(".expense-name");
    var amount = document.querySelector(".expense-amount");
    var addButton = document.querySelector(".add-expense");

    if (addButton) {
        addButton.addEventListener("click", function (event) {
            var nameValue = expenseName ? expenseName.value.trim() : "";
            var amountValue = amount ? amount.value.trim() : "";
            var amountNumber = Number(amountValue);

            if (nameValue === "") {
                event.preventDefault();
                alert("Please enter the expense name.");
                if (expenseName) {
                    expenseName.focus();
                }
                return;
            }

            if (
                amountValue === "" ||
                !Number.isFinite(amountNumber) ||
                amountNumber <= 0
            ) {
                event.preventDefault();
                alert("Please enter a valid positive amount.");
                if (amount) {
                    amount.focus();
                }
            }
        });
    }

    document.addEventListener("click", function (event) {
        var target = event.target;

        if (target.tagName === "A" &&
            target.textContent.trim().toLowerCase() === "delete") {
            var confirmed = confirm("Are you sure you want to delete this expense?");

            if (!confirmed) {
                event.preventDefault();
                event.stopPropagation();
            }
        }
    });
});
