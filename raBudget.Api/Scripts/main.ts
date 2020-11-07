import 'bootstrap-material-design/dist/css/bootstrap-material-design.min.css';
import '../Styles/main.scss';
import { MDCTextField } from '@material/textfield';
import { MDCRipple } from '@material/ripple';
import { MDCFormField } from '@material/form-field';
import { MDCCheckbox } from '@material/checkbox';
import { MDCTextFieldHelperText } from '@material/textfield/helper-text';

window['MDCTextField'] = MDCTextField;
window['MDCCheckbox'] = MDCCheckbox;
window['MDCFormField'] = MDCFormField;

document.addEventListener('load', () => {
    
    document.querySelectorAll('.mdc-button').forEach(button => new MDCRipple(button));
    document.querySelectorAll('.mdc-text-field-helper-text').forEach(helperText => new MDCTextFieldHelperText(helperText));
    
});