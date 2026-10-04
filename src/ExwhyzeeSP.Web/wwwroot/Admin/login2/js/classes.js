/*
	*	By: Nwachukwu Daniel
	*	donsn54@yahoo.com

	*	Description:
	*	Inserts Nigerian Class System as Dropdown List
	*	How to Use:

		In Head/Footer section:
		<script type= "text/javascript" src = "classes.js"></script>
		In Body Section:
		Select Country:   <select id="classes" name ="classes"></select>
		<script language="javascript">populate_classlist("classes");</script>	
	*
	*	Author's Website: https://www.madtins.com/
	*
*/

var classes_arr = new Array("Nursery 1","Nursery 2","Primary 1","Primary 2","Primary 3","Primary 4","Primary 5","Primary 6","JSS 1","JSS 2", "JSS 3","SSS 1", "SSS 2", "SSS 3");


function populate_classlist(class_id){
	// given the id of the <select> tag as function argument, it inserts <option> tags
	var option_str = document.getElementById(class_id);
	option_str.length=0;
	option_str.options[0] = new Option('Select Class','');
	option_str.selectedIndex = 0;
	for (var i=0; i<country_arr.length; i++) {
		option_str.options[option_str.length] = new Option(classes_arr[i],classes_arr[i]);
	}
}
