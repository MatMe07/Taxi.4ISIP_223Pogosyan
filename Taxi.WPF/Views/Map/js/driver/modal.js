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
