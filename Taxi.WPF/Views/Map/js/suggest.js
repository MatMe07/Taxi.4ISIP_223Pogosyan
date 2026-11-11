function initSuggest(inputId, listId, placeholderType) {
    const input = document.getElementById(inputId);
    const suggestList = document.getElementById(listId);
    // console.log(input);

    if (!input || !suggestList) return;

    async function updateSuggests(text) {

        try {

            const url = `https://suggest-maps.yandex.ru/v1/suggest?apikey=30756955-06ac-4ef6-8412-dc34183e9b85&text=${text}&print_address=1&attrs=uri&ll=${addressUser.lon},${addressUser.lat}`;
            // console.log("uri - userPoS" + JSON.stringify(addressUser));
            const response = await fetch(url);
            const data = await response.json();

            renderSuggests(data.results);
        } catch (error) {
            console.error(`Ошибка саджеста для ${inputId}:`, error);
        }
    }

    function renderSuggests(items) {
        suggestList.innerHTML = '';

        if (items.length === 0) {
            suggestList.style.display = 'none';
            return;
        }

        items.forEach(item => {
            // console.info(item);
            const li = document.createElement('li');
            li.className = 'suggest-item';



            const value = item.title.text;
            const subtitle = item.subtitle ? `${item.subtitle.text}` : '';
            const distance = item.distance.text;
            //   li.textContent = value;

            li.innerHTML += `
                <div class="suggest-content">
            <div class="suggest-title">
                ${value}
            </div>
            <div class="suggest-subtitle">
                ${subtitle}
            </div>
        </div>

        <div class="suggest-distance">
            ${distance}
        </div>
        <div class="suggest-icon">
            <svg width="15"  viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M9 18L15 12L9 6" stroke="#21201F" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
        </div>
      `;

            li.addEventListener('click', () => {
                input.value = value;
                suggestList.style.display = 'none';
                onSelectAddress(item, placeholderType);
            });

            suggestList.appendChild(li);
        });

        suggestList.style.display = 'block';
    }

    input.addEventListener('input', debounce((e) => {
        updateSuggests(e.target.value);
    }, 100));

    document.addEventListener('click', (e) => {
        if (e.target !== input) {
            suggestList.style.display = 'none';
        }
    });
}



async function onSelectAddress(suggestItem, type) {
    const fullAddress = `${suggestItem.title.text} ${suggestItem.subtitle?.text || ''}`;
    if (type === 'from') {
        addressOtkuda = suggestItem.title.text;
        postToHost({ type: 'destinationSelectedOtkuda', address: fullAddress, uri: suggestItem.uri });

    } else if (type === 'to') {
        postToHost({ type: 'destinationSelectedKuda', address: fullAddress, uri: suggestItem.uri });
    }
    showTariffsSkeleton();

}
