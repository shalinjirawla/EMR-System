import { definePreset } from '@primeng/themes';
import Lara from '@primeng/themes/lara';
import Material from '@primeng/themes/material';
import Aura from '@primeng/themes/aura';


const MyPreset = definePreset(Lara, {
  semantic: {
    primary: {
      50: '{teal.50}',
      100: '{teal.100}',
      200: '{teal.200}',
      300: '{teal.300}',
      400: '{teal.400}',
      500: '{teal.500}',
      600: '{teal.600}',
      700: '{teal.700}',
      800: '{teal.800}',
      900: '{teal.900}',
      950: '{teal.950}',
    },
    colorScheme: {
      light: {
        primary: {
          color: '{teal.700}',           // Button/Link color
          inverseColor: '#ffffff',
          hoverColor: '{teal.800}',
          activeColor: '{teal.900}',
        },
        highlight: {
          background: '{teal.50}',        // Input focus, table hover
          focusBackground: '{teal.100}',
          color: '{teal.900}',
          focusColor: '{teal.900}',
        },
      },
      dark: {
        primary: {
          color: '{teal.200}',
          inverseColor: '#0f172a',
          hoverColor: '{teal.100}',
          activeColor: '{teal.50}',
        },
        highlight: {
          background: 'rgba(94, 234, 212, 0.16)',
          focusBackground: 'rgba(94, 234, 212, 0.24)',
          color: 'rgba(255,255,255,.87)',
          focusColor: 'rgba(255,255,255,.87)',
        },
      },
    },
  },
});

export default MyPreset;
