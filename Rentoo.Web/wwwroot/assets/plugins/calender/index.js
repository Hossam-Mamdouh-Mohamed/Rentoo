const configurePicker = (elementRef, calendarType, lang,mindatePar=undefined,maxdatePar=undefined) => {

    const calendar = $.calendars.instance(calendarType, lang); 
    const gregCal = $.calendars.instance("gregorian");
    let gregMin, gregMax;
    if (mindatePar!=undefined&&maxdatePar!=undefined) {

        gregMin = gregCal.newDate(mindatePar.split(/,\s*/).map(Number)[0], mindatePar.split(/,\s*/).map(Number)[1], mindatePar.split(/,\s*/).map(Number)[2]); 
        gregMax = gregCal.newDate(maxdatePar.split(/,\s*/).map(Number)[0], maxdatePar.split(/,\s*/).map(Number)[1], maxdatePar.split(/,\s*/).map(Number)[2]); 

    }
    let minDate, maxDate;
    try {
        minDate = calendar.fromJD(gregMin.toJD());
        maxDate = calendar.fromJD(gregMax.toJD());
    } catch (e) {
        if (mindatePar != undefined && maxdatePar != undefined) {

        minDate = calendar.newDate(mindatePar.split(/,\s*/).map(Number)[0], mindatePar.split(/,\s*/).map(Number)[1], mindatePar.split(/,\s*/).map(Number)[2]); // Fallback: 1445 AH
        maxDate = calendar.newDate(maxdatePar.split(/,\s*/).map(Number)[0], maxdatePar.split(/,\s*/).map(Number)[1], maxdatePar.split(/,\s*/).map(Number)[2]); // Fallback: 1445 AH
    }
    }
    $(elementRef).calendarsPicker({
        calendar: $.calendars.instance(calendarType, lang),
        popupContainer: document.getElementById('container'),
        dateFormat: "yyyy/mm/dd",
        minDate: minDate, // Set converted min date
        maxDate:maxDate, // Set converted max date

        showOtherMonths: true,
        nextText: '<i class="hgi-stroke hgi-sharp hgi-arrow-left-02"></i>',
        prevText: '<i class="hgi-stroke hgi-sharp hgi-arrow-right-02"></i>',
        renderer: {
            weekendClass: '',
            picker: '<div class="calendars">' +
                '' +
                '{months}' +
                '{popup:start}' +
                '{popup:end}' +
                '<div class="calendars-clear-fix"></div></div>',
            month: '<div class="calendars-month">' +
                '<div class="calendar-controls">' +
                '<div class="calendars-month-header">{monthHeader}</div>' +
                '<div class="calendars-nav">{button:prev}{button:next}</div>' +
                '</div>' +
                '<table><thead>{weekHeader}</thead><tbody>{weeks}</tbody></table></div>',
        },
        onSelect: (dates) => {
            //debugger;
            //let otherCalendarType = 'gregorian';
            //if (calendarType == otherCalendarType) {
            //    otherCalendarType = 'ummalqura';
            //}
            //let otherRef = elementRef.replace(calendarType, otherCalendarType);
            //let otherCalendar = $.calendars.instance(otherCalendarType, lang);
            //let otherDate = otherCalendar.fromJD(dates[0].toJD());
            //debugger;
            //$/*(elementRef).closest*/(otherRef).val(otherCalendar.formatDate("yyyy/mm/dd", otherDate));
            //onSelectCallback(dates);
        },
    }).on("change", (e) => {
            const dates = $(e.target.parentNode.childNodes).closest(elementRef).calendarsPicker("getDate");
            let otherCalendarType = 'gregorian';
            if (calendarType == otherCalendarType) {
                otherCalendarType = 'ummalqura';
            }
            let otherRef = elementRef.replace(calendarType, otherCalendarType);
            let otherCalendar = $.calendars.instance(otherCalendarType, lang);
            if (dates.length > 0) {
                let otherDate = otherCalendar.fromJD(dates[0].toJD());
                $(e.target.parentNode.childNodes).closest(otherRef).val(otherCalendar.formatDate("yyyy/mm/dd", otherDate));
            }
        });
}

$(document).ready(function () {
    configurePicker('.date-ar-gregorian', "gregorian", "ar");
    configurePicker('.date-ar-ummalqura', "ummalqura", "ar");
    configurePicker('.date-en-gregorian', "gregorian", "en");
    configurePicker('.date-en-ummalqura', "ummalqura", "en");
});
