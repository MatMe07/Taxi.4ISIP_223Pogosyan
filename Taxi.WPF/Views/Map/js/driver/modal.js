function btnModalCancelOpen(){
    document.querySelector('.ModalCancel').classList.add("open");
}

function btnCloseModalCancel() {
    document.querySelector('.ModalCancel').classList.remove("open");
    // console.info(selectedPaymentMethod);
}

function closeModalCancel(){
    btnCloseModalCancel();
    setDriverState(DRIVER_STATE.CANCELED);
}

function ShowSnackbar(message) {
    var x = document.getElementById("snackbar");
    x.innerHTML = `<span>${message}</span>`;
    x.className = "show";
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}
