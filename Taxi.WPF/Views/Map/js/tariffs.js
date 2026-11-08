class Tariff {
    constructor(id, title, price, time, imgPath, isRoute) {
        this.id = id;
        this.title = title;
        this.price = price;
        this.time = time;
        this.imgPath = imgPath;
        this.isRoute = isRoute;
    }
}

var tariffsList = [
    new Tariff(1, 'Эконом', 339, '12 мин', 'imgs/classTariff/ekonomClass.png'),
    new Tariff(2, 'Комфорт', 450, '7 мин', 'imgs/classTariff/komfort.png'),
    new Tariff(3, 'Бизнес', 790, '4 мин', 'imgs/classTariff/business.png'),
    new Tariff(4, 'Минивэн', 340, '5 мин', 'imgs/classTariff/miniven.png')
];


function inti_Traffics() {
    const tariffContainer = document.querySelector(".tariffWrapper");
    showTariffsSkeleton();
    tariffContainer.addEventListener('click', (event) => {
        const clickedButton = event.target.closest('.btnTariffs');
        if (!clickedButton) return;
        selectTariff = tariffsList.find(t => t.id == clickedButton.id);
        console.info(selectTariff);

        tariffContainer.querySelectorAll('.btnTariffs').forEach(btn => {
            btn.setAttribute('aria-pressed', 'false');
            btn.setAttribute('aria-checked', 'false');
            btn.classList.remove('active');
        });
        clickedButton.setAttribute('aria-pressed', 'true');
        clickedButton.setAttribute('aria-checked', 'true');
        clickedButton.classList.add('active');
    });



    // for (let index = 0; index < tariffsList.length; index++) {


    // }

}

function UpdateTariff() {
    const tariffContainer = document.querySelector(".tariffWrapper");
    tariffContainer.innerHTML = '';
    console.info("VIZZOOVVV");
    console.info(tariffsList);
    tariffsList.forEach(tarif => {
        console.info(tarif);
        if (!tarif.isRoute) {
            const disabledButtonHTML = `
        <button role="radio" aria-pressed="false" aria-checked="false" id="${tarif.id}" class="btnTariffs disabled" disabled style="opacity: 0.6; cursor: not-allowed;">
            <span style="flex-shrink: 0;" class="box--img">
                <img src="${tarif.imgPath}" alt="${tarif.title}" width="56" height="20" class="car-img" style="filter: grayscale(100%);">
                <span class="etaTimes"> — </span>
            </span>
            <span class="content">
                <span class="title" style="color: #888;">${tarif.title}</span>
                <span class="price--WnKzJ">
                    <span class="priceText--YA8Je" style="font-size: 12px; color: #999;">Недоступно</span>
                </span>
            </span>
        </button>
        `;
            tariffContainer.insertAdjacentHTML('beforeend', disabledButtonHTML);
        } else {

            const buttonHTML = `
            <button role="radio" aria-pressed="false" aria-checked="false" id="${tarif.id}" class="btnTariffs ${tarif.title == 'Эконом' ? 'active' : ''}">
            <span style="flex-shrink: 0;" class="box--img">
            <img src=${tarif.imgPath} alt="Желтая машина" width="56" height="20" class="car-img">
            <span class="etaTimes">${tarif.time == "" ? " — " : tarif.time}</span>
            </span>
            <span class="content">
            <span class="title">${tarif.title}</span>
            <span class="price--WnKzJ">
            <span class="priceSurge--jcKcC"></span>
            <span class="priceText--YA8Je">от ${Math.trunc(tarif.price)} ₽</span>
            </span>
            </span>
            </button>
            `;

            tariffContainer.insertAdjacentHTML('beforeend', buttonHTML);
        }
    });
}


function showTariffsSkeleton() {
    const tariffContainer = document.querySelector(".tariffWrapper");
    
    tariffContainer.innerHTML = '';
    
    for (let i = 0; i < 4; i++) {
        const skeletonHTML = `
        <button role="radio" class="btnTariffs skeleton" disabled style="pointer-events: none;">
            <span style="flex-shrink: 0;" class="box--img">
            </span>
        </button>
        `;
        tariffContainer.insertAdjacentHTML('beforeend', skeletonHTML);
    }
}
