(function($) {
	'use strict';
	$.calendars.calendars.gregorian.prototype.regionalOptions['ar'] = {
		name: 'Gregorian',
		epochs: ['BCE', 'CE'],
		monthNames: ['يناير', 'فبراير', 'مارس', 'إبريل', 'مايو', 'يونيو',
		'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'],
		monthNamesShort: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'],
		dayNames:  ['الأحد', 'الاثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'],
		dayNamesShort: ['أحد', 'اثنين', 'ثلاثاء', 'أربعاء', 'خميس', 'جمعة', 'سبت'],
		dayNamesMin: ['أحد', 'اثنين', 'ثلاثاء', 'أربعاء', 'خميس', 'جمعة', 'سبت'],
		digits: null,
		dateFormat: 'dd/mm/yyyy',
		firstDay: 6,
		isRTL: true
	};
	
    $.calendars.calendars.ummalqura.prototype.regionalOptions.ar = {
		name: 'UmmAlQura', // The calendar name
		epochs: ['BAM', 'AM'],
		monthNames: 'محرم_صفر_ربيع الأول_ربيع الثاني_جمادى الأول_جمادى الآخر_رجب_شعبان_رمضان_شوال_ذو القعدة_ذو الحجة'.split('_'),
		monthNamesShort: 'محرم_صفر_ربيع1_ربيع2_جمادى1_جمادى2_رجب_شعبان_رمضان_شوال_القعدة_الحجة'.split('_'),
		dayNames: ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'],
		dayNamesShort: 'أحد_إثنين_ثلاثاء_أربعاء_خميس_جمعة_سبت'.split('_'),
		dayNamesMin: 'ح_ن_ث_ر_خ_ج_س'.split('_'),
		digits: $.calendars.substituteDigits(['1', '2', '3', '4', '5', '6', '7', '8', '9']),
		dateFormat: 'yyyy/mm/dd',
		firstDay: 1,
		isRTL: true
	};
})(jQuery);